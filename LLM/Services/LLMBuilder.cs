using Microsoft.Extensions.DependencyInjection;
using LLM.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LLM.Attributes;
using System.Reflection;
using System.Linq.Expressions;
using System.Linq;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Net.Http;
using LLM.Utilities;

namespace LLM.Services
{
    public class LLMBuilder
    {
        private readonly List<Type> _functionTypes = new List<Type>();
        private readonly List<Delegate> _functionDelegates = new List<Delegate>();
        private readonly List<MessageBase> _initialMessages = new List<MessageBase>();
        private List<object> _instances = [];
        private string _baseUrl = "http://127.0.0.1:11434";
        private string _model = "qwen2.5:3b";
        private string _format = "";
        private string? _bearer;
        private LLMType _lLMType;
        private object _options = new { };
        private object? _keepAlive = null;
        private HttpClient? _httpClient;


        public LLMBuilder SetBaseUrl(string apiUrl)
        {
            _baseUrl = apiUrl;
            return this;
        }

        public LLMBuilder SetHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
            return this;
        }

        public LLMBuilder SetBearer(string bearer)
        {
            _bearer = bearer;
            return this;
        }
        public LLMBuilder SetLLMType(LLMType lLMType)
        {
            _lLMType = lLMType;
            return this;
        }

        public LLMBuilder SetModel(string model)
        {
            _model = model;
            return this;
        }

        public LLMBuilder AddTool<T>()
        {
            _functionTypes.Add(typeof(T));
            return this;
        }

        public LLMBuilder AddTool(Delegate action)
        {
            _functionDelegates.Add(action);
            return this;
        }


        /// <summary>
        /// 通过动态代码添加工具
        /// </summary>
        public LLMBuilder AddTool(string code, object? target = null)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("代码不能为空。", nameof(code));

            // 初始化动态编译器
            var compiler = new DynamicFunctionCompiler();

            // 编译函数
            Assembly? assembly;
            (var error, assembly) = compiler.CompileFunction(code);

            if (error.Count > 0)
            {
                throw new InvalidOperationException("代码编译失败。");

            }
            else
            {

                // 获取程序集中的所有导出类型
                var exportedTypes = assembly!.ExportedTypes.ToList();
                if (exportedTypes.Count == 0)
                    throw new InvalidOperationException("编译后的程序集不包含任何类型。");

                foreach (var type in exportedTypes)
                {
                    // 获取标记了 FunctionDescriptionAttribute 的公共方法（静态和实例）
                    var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
                                      .Where(m => m.GetCustomAttribute<FunctionDescriptionAttribute>() != null)
                                      .ToList();

                    if (!methods.Any())
                        continue;

                    // 如果存在实例方法且未提供目标对象，尝试创建实例
                    object? instance = null;
                    var instanceMethods = methods.Where(m => !m.IsStatic).ToList();
                    if (instanceMethods.Any())
                    {
                        if (target != null)
                        {
                            instance = target;
                        }
                        else
                        {
                            try
                            {
                                instance = Activator.CreateInstance(type);
                                if (instance == null)
                                {
                                    throw new InvalidOperationException($"无法创建类型 {type.FullName} 的实例。");
                                }
                                _instances.Add(instance); // 存储实例，防止被垃圾回收
                            }
                            catch (Exception ex)
                            {
                                throw new InvalidOperationException($"无法创建类型 {type.FullName} 的实例。", ex);
                            }
                        }
                    }

                    foreach (var method in methods)
                    {
                        // 判断方法是否为静态方法
                        bool isStatic = method.IsStatic;

                        // 如果是实例方法但未提供目标对象，使用自动创建的实例
                        object? methodTarget = isStatic ? null : instance;

                        // 获取方法的参数类型和返回类型
                        var parameterTypes = method.GetParameters()
                                                   .Select(p => p.ParameterType)
                                                   .ToArray();

                        // 包含返回类型在内的所有类型，用于创建委托类型
                        Type delegateType;
                        try
                        {
                            delegateType = Expression.GetDelegateType(
                                parameterTypes.Concat(new[] { method.ReturnType }).ToArray());
                        }
                        catch (Exception ex)
                        {
                            throw new InvalidOperationException($"无法为方法 {method.Name} 创建委托类型。", ex);
                        }

                        // 创建委托实例
                        Delegate del;
                        try
                        {
                            del = method.CreateDelegate(delegateType, methodTarget);
                        }
                        catch (Exception ex)
                        {
                            throw new InvalidOperationException($"无法为方法 {method.Name} 创建委托实例。", ex);
                        }

                        // 添加到委托集合中
                        _functionDelegates.Add(del);
                    }
                }


            }

            return this;
        }


        private readonly DynamicFunctionCompiler _compiler = new();
        /// <summary>
        /// 通过动态代码添加工具
        /// </summary>


        // 缓存：代码哈希 -> 委托列表
        private static readonly ConcurrentDictionary<string, List<Delegate>> _delegateCache = new ConcurrentDictionary<string, List<Delegate>>();

        /// <summary>
        /// 计算字符串的 SHA256 哈希值
        /// </summary>
        private string ComputeHash(string input)
        {
            byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
            StringBuilder builder = new();
            foreach (var b in bytes)
                builder.Append(b.ToString("x2"));
            return builder.ToString();
        }

        public LLMBuilder AddTool(List<string> codes)
        {
            foreach (var code in codes)
            {
                // 计算代码的哈希值
                string codeHash = ComputeHash(code);

                // 检查缓存中是否已有对应的委托
                if (_delegateCache.TryGetValue(codeHash, out List<Delegate>? cachedDelegates))
                {
                    // 使用缓存的委托
                    _functionDelegates.AddRange(cachedDelegates);
                    continue;
                }

                try
                {
                    // 编译函数
                    (var error, Assembly? assembly) = _compiler.CompileFunction(code);

                    if (error.Count == 0 && assembly != null)
                    {
                        var delegates = new List<Delegate>();

                        // 获取程序集中的所有导出类型
                        var exportedTypes = assembly.ExportedTypes.ToList();
                        if (exportedTypes.Any())
                        {
                            foreach (var type in exportedTypes)
                            {
                                // 获取标记了 FunctionDescriptionAttribute 的公共方法（静态和实例）
                                var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
                                                  .Where(m => m.GetCustomAttribute<FunctionDescriptionAttribute>() != null)
                                                  .ToList();

                                if (!methods.Any())
                                    continue;

                                // 如果存在实例方法且未提供目标对象，尝试创建实例
                                object? instance = null;
                                var instanceMethods = methods.Where(m => !m.IsStatic).ToList();
                                if (instanceMethods.Any())
                                {
                                    try
                                    {
                                        instance = Activator.CreateInstance(type);
                                        if (instance == null)
                                        {
                                            throw new InvalidOperationException($"无法创建类型 {type.FullName} 的实例。");
                                        }
                                        _instances.Add(instance); // 存储实例，防止被垃圾回收
                                    }
                                    catch (Exception ex)
                                    {
                                        throw new InvalidOperationException($"无法创建类型 {type.FullName} 的实例。", ex);
                                    }
                                }

                                foreach (var method in methods)
                                {
                                    // 判断方法是否为静态方法
                                    bool isStatic = method.IsStatic;

                                    // 如果是实例方法但未提供目标对象，使用自动创建的实例
                                    object? methodTarget = isStatic ? null : instance;

                                    // 获取方法的参数类型和返回类型
                                    var parameterTypes = method.GetParameters()
                                                               .Select(p => p.ParameterType)
                                                               .ToArray();

                                    // 包含返回类型在内的所有类型，用于创建委托类型
                                    Type delegateType;
                                    try
                                    {
                                        delegateType = Expression.GetDelegateType(
                                            parameterTypes.Concat(new[] { method.ReturnType }).ToArray());
                                    }
                                    catch (Exception ex)
                                    {
                                        throw new InvalidOperationException($"无法为方法 {method.Name} 创建委托类型。", ex);
                                    }

                                    // 创建委托实例
                                    Delegate del;
                                    try
                                    {
                                        del = method.CreateDelegate(delegateType, methodTarget);
                                    }
                                    catch (Exception ex)
                                    {
                                        throw new InvalidOperationException($"无法为方法 {method.Name} 创建委托实例。", ex);
                                    }

                                    // 添加到本地委托列表
                                    delegates.Add(del);
                                }
                            }
                        }

                        // 将创建的委托添加到全局和缓存中
                        _functionDelegates.AddRange(delegates);
                        _delegateCache.TryAdd(codeHash, delegates);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }


            return this;
        }
        public LLMBuilder AddChatHistory(List<ChatMessage> chatMessages)
        {
            switch (_lLMType)
            {
                case LLMType.Ollama:
                    {
                        foreach (ChatMessage chatMessage in chatMessages)
                        {
                            if (chatMessage.IsBot)
                            {
                                _initialMessages.Add(new Models.Ollama.Message { Role = "assistant", Content = chatMessage.Content ?? "" });
                            }
                            else
                            {
                                _initialMessages.Add(new Models.Ollama.Message { Role = "user", Content = chatMessage.Content ?? "" });
                            }
                        }
                    }
                    break;
                case LLMType.豆包:
                    break;
                case LLMType.通义千问:
                    break;
                case LLMType.ChatGpt:
                    foreach (ChatMessage chatMessage in chatMessages)
                    {
                        if (chatMessage.IsBot)
                        {
                            _initialMessages.Add(new Models.ChatGpt.Message { role = "assistant", content = chatMessage.Content ?? "" });
                        }
                        else
                        {
                            _initialMessages.Add(new Models.ChatGpt.Message { role = "user", content = chatMessage.Content ?? "" });
                        }
                    }
                    break;
                default:
                    break;
            }

            return this;
        }

        public LLMBuilder AddSystemMessage(string content)
        {
            switch (_lLMType)
            {
                case LLMType.Ollama:
                    _initialMessages.Add(new Models.Ollama.Message { Role = "system", Content = content });
                    break;
                case LLMType.豆包:
                    _initialMessages.Add(new Models.DouBao.Message { Role = "system", Content = content });
                    break;
                case LLMType.通义千问:
                    _initialMessages.Add(new Models.QianWen.Message { Role = "system", Content = content });
                    break;
                case LLMType.ChatGpt:
                    _initialMessages.Add(new Models.ChatGpt.Message { role = "system", content = content });
                    break;
                default:
                    break;
            }
            return this;
        }


        public LLMClient Build()
        {
            return new LLMClient(new LLMClient.LLMOption
            {
                BaseUrl = _baseUrl,
                Model = _model,
                Format = _format,
                LLMType = _lLMType,
                Options = _options,
                KeepAlive = _keepAlive,
                FunctionTypes = _functionTypes,
                FunctionDelegates = _functionDelegates,
                InitialMessages = _initialMessages,
                Bearer = _bearer,

            });
        }
    }
}
