using LLM.Attributes;
using LLM.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LLM.Utilities.Ollama
{
    public class ReflectionHelper
    {

        public static void AddMethodsAsTools(Type type, List<ToolBase> tools, LLMType lLMType)
        {
            var methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public);

            foreach (var method in methods)
            {
                // 获取方法描述
                var funcDescAttr = method.GetCustomAttribute<FunctionDescriptionAttribute>();
                var funcDescription = funcDescAttr != null ? funcDescAttr.Description : "";

                // 构建参数信息
                var parameters = method.GetParameters();

                switch (lLMType)
                {
                    case LLMType.Ollama:
                        {
                            var parameterDict = new Dictionary<string, Models.Ollama.Property>();
                            var requiredParams = new List<string>();

                            foreach (var param in parameters)
                            {
                                // 获取参数描述
                                var paramDescAttr = param.GetCustomAttribute<ParameterDescriptionAttribute>();
                                var paramDescription = paramDescAttr != null ? paramDescAttr.Description : "";

                                // 参数类型转换为 JSON Schema 类型
                                string paramType = TypeHelper.GetJsonSchemaType(param.ParameterType);

                                parameterDict.Add(param.Name!, new Models.Ollama.Property
                                {
                                    Type = paramType,
                                    Description = paramDescription
                                });

                                requiredParams.Add(param.Name!);
                            }

                            // 构建 Function 对象
                            var function = new Models.Ollama.Function
                            {
                                Name = method.Name,
                                Description = funcDescription,
                                Parameters = new Models.Ollama.Parameters
                                {
                                    Type = "object",
                                    Properties = parameterDict,
                                    Required = requiredParams
                                }
                            };

                            // 添加到工具列表
                            tools.Add(new Models.Ollama.Tool
                            {
                                Type = "function",
                                Function = function
                            });
                        }
                        break;
                    case LLMType.豆包:
                        {
                            var parameterDict = new Dictionary<string, Models.Ollama.Property>();
                            var requiredParams = new List<string>();

                            foreach (var param in parameters)
                            {
                                // 获取参数描述
                                var paramDescAttr = param.GetCustomAttribute<ParameterDescriptionAttribute>();
                                var paramDescription = paramDescAttr != null ? paramDescAttr.Description : "";

                                // 参数类型转换为 JSON Schema 类型
                                string paramType = TypeHelper.GetJsonSchemaType(param.ParameterType);

                                parameterDict.Add(param.Name!, new Models.Ollama.Property
                                {
                                    Type = paramType,
                                    Description = paramDescription
                                });

                                requiredParams.Add(param.Name!);
                            }

                            // 构建 Function 对象
                            var function = new Models.Ollama.Function
                            {
                                Name = method.Name,
                                Description = funcDescription,
                                Parameters = new Models.Ollama.Parameters
                                {
                                    Type = "object",
                                    Properties = parameterDict,
                                    Required = requiredParams
                                }
                            };

                            // 添加到工具列表
                            tools.Add(new Models.Ollama.Tool
                            {
                                Type = "function",
                                Function = function
                            });
                        }
                        break;
                    case LLMType.通义千问:
                        {

                        }
                        break;
                    case LLMType.ChatGpt:
                        {

                            var parameterDict = new Dictionary<string, object>();
                            var requiredParams = new List<string>();

                            foreach (var param in parameters)
                            {
                                // 获取参数描述
                                var paramDescAttr = param.GetCustomAttribute<ParameterDescriptionAttribute>();
                                var paramDescription = paramDescAttr != null ? paramDescAttr.Description : "";

                                // 参数类型转换为 JSON Schema 类型
                                string paramType = TypeHelper.GetJsonSchemaType(param.ParameterType);

                                parameterDict.Add(param.Name!, new
                                {
                                    type = paramType,
                                    description = paramDescription
                                });

                                requiredParams.Add(param.Name!);
                            }

                            // 构建 Function 对象
                            var function = new Models.ChatGpt.Function
                            {

                                name = method.Name,
                                description = funcDescription,
                                parameters = new Models.ChatGpt.Parameters
                                {
                                    type = "object",
                                    properties = parameterDict,
                                    required = requiredParams
                                }
                            };

                            // 添加到工具列表
                            tools.Add(new Models.ChatGpt.Tool
                            {
                                type = "function",
                                function = function
                            });
                        }
                        break;
                    default:
                        break;
                }


            }
        }






        public static void AddMethodsAsTools(Delegate @delegate, List<ToolBase> tools, LLMType lLMType)
        {
            if (@delegate == null)
                throw new ArgumentNullException(nameof(@delegate));
            if (tools == null)
                throw new ArgumentNullException(nameof(tools));

            // 获取委托关联的方法信息
            MethodInfo method = @delegate.Method;

            // 获取方法描述
            var funcDescAttr = method.GetCustomAttribute<FunctionDescriptionAttribute>();
            var funcDescription = funcDescAttr != null ? funcDescAttr.Description : "";

            // 构建参数信息
            var parameters = method.GetParameters();




            switch (lLMType)
            {
                case LLMType.Ollama:
                    {
                        var parameterDict = new Dictionary<string, Models.Ollama.Property>();
                        var requiredParams = new List<string>();

                        foreach (var param in parameters)
                        {
                            // 获取参数描述
                            var paramDescAttr = param.GetCustomAttribute<ParameterDescriptionAttribute>();
                            var paramDescription = paramDescAttr != null ? paramDescAttr.Description : "";

                            // 参数类型转换为 JSON Schema 类型
                            string paramType = TypeHelper.GetJsonSchemaType(param.ParameterType);

                            parameterDict.Add(param.Name!, new Models.Ollama.Property
                            {
                                Type = paramType,
                                Description = paramDescription
                            });

                            requiredParams.Add(param.Name!);
                        }

                        // 构建 Function 对象
                        var function = new Models.Ollama.Function
                        {
                            Name = funcDescAttr?.Name ?? method.Name,
                            Description = funcDescription,
                            Parameters = new Models.Ollama.Parameters
                            {
                                Type = "object",
                                Properties = parameterDict,
                                Required = requiredParams
                            }
                        };

                        // 添加到工具列表
                        tools.Add(new Models.Ollama.Tool
                        {
                            Type = "function",
                            Function = function
                        });
                    }
                    break;
                case LLMType.豆包:
                    {
                        var parameterDict = new Dictionary<string, Models.DouBao.Property>();
                        var requiredParams = new List<string>();

                        foreach (var param in parameters)
                        {
                            // 获取参数描述
                            var paramDescAttr = param.GetCustomAttribute<ParameterDescriptionAttribute>();
                            var paramDescription = paramDescAttr != null ? paramDescAttr.Description : "";

                            // 参数类型转换为 JSON Schema 类型
                            string paramType = TypeHelper.GetJsonSchemaType(param.ParameterType);

                            parameterDict.Add(param.Name!, new Models.DouBao.Property
                            {
                                Type = paramType,
                                Description = paramDescription
                            });

                            requiredParams.Add(param.Name!);
                        }

                        // 构建 Function 对象
                        var function = new Models.DouBao.Function
                        {
                            Name = funcDescAttr?.Name ?? method.Name,
                            Description = funcDescription,
                            Parameters = new Models.DouBao.Parameters
                            {
                                Type = "object",
                                Properties = parameterDict,
                                Required = requiredParams
                            }
                        };

                        // 添加到工具列表
                        tools.Add(new Models.DouBao.Tool
                        {
                            Type = "function",
                            Function = function
                        });
                    }
                    break;
                case LLMType.通义千问:
                    {
                        var parameterDict = new Dictionary<string, Models.QianWen.Property>();
                        var requiredParams = new List<string>();

                        foreach (var param in parameters)
                        {
                            // 获取参数描述
                            var paramDescAttr = param.GetCustomAttribute<ParameterDescriptionAttribute>();
                            var paramDescription = paramDescAttr != null ? paramDescAttr.Description : "";

                            // 参数类型转换为 JSON Schema 类型
                            string paramType = TypeHelper.GetJsonSchemaType(param.ParameterType);

                            parameterDict.Add(param.Name!, new Models.QianWen.Property
                            {
                                Type = paramType,
                                Description = paramDescription
                            });

                            requiredParams.Add(param.Name!);
                        }

                        // 构建 Function 对象
                        var function = new Models.QianWen.Function
                        {
                            Name = funcDescAttr?.Name ?? method.Name,
                            Description = funcDescription,
                            Parameters = new Models.QianWen.Parameters
                            {
                                Type = "object",
                                Properties = parameterDict,
                                Required = requiredParams
                            }
                        };

                        // 添加到工具列表
                        tools.Add(new Models.QianWen.Tool
                        {
                            Type = "function",
                            Function = function
                        });
                    }
                    break;
                case LLMType.ChatGpt:
                    {

                        var parameterDict = new Dictionary<string, object>();
                        var requiredParams = new List<string>();

                        foreach (var param in parameters)
                        {
                            // 获取参数描述
                            var paramDescAttr = param.GetCustomAttribute<ParameterDescriptionAttribute>();
                            var paramDescription = paramDescAttr != null ? paramDescAttr.Description : "";

                            // 参数类型转换为 JSON Schema 类型
                            string paramType = TypeHelper.GetJsonSchemaType(param.ParameterType);

                            parameterDict.Add(param.Name!, new
                            {
                                type = paramType,
                                description = paramDescription
                            });

                            requiredParams.Add(param.Name!);
                        }

                        // 构建 Function 对象
                        var function = new Models.ChatGpt.Function
                        {

                            name = funcDescAttr?.Name ?? method.Name,
                            description = funcDescription,
                            parameters = new Models.ChatGpt.Parameters
                            {
                                type = "object",
                                properties = parameterDict,
                                required = requiredParams
                            }
                        };

                        // 添加到工具列表
                        tools.Add(new Models.ChatGpt.Tool
                        {
                            type = "function",
                            function = function
                        });
                    }
                    break;
                default:
                    break;
            }


        }



        public async Task<object?> ExecuteFunctionsFromToolCallAsync(List<Type> types, string functionName, Dictionary<string, string> arguments)
        {
            foreach (var type in types)
            {
                // 获取要调用的方法
                var method = type.GetMethod(functionName, BindingFlags.Static | BindingFlags.Public);

                if (method == null)
                {
                    continue;
                }

                // 获取方法的参数信息
                var parameters = method.GetParameters();
                var parameterValues = new object[parameters.Length];

                for (int i = 0; i < parameters.Length; i++)
                {
                    var param = parameters[i];
                    if (arguments.TryGetValue(param.Name!, out var argValue))
                    {
                        if (!string.IsNullOrWhiteSpace(argValue))
                        {
                            parameterValues[i] = Convert.ChangeType(argValue, param.ParameterType);
                        }
                        else
                        {
                            return $"{param.Name} 参数需指明!";
                        }
                    }
                    else
                    {
                        Console.WriteLine($"参数 {param.Name} 未提供。");
                        parameterValues[i] = GetDefault(param.ParameterType) ?? new object();
                    }
                }

                try
                {
                    // 调用方法
                    var result = method.Invoke(null, parameterValues);

                    // 检查方法是否返回 Task，即是否为异步方法
                    if (result is Task task)
                    {
                        await task.ConfigureAwait(false);

                        // 如果是 Task<T>，获取结果
                        if (result.GetType().IsGenericType)
                        {
                            var property = result.GetType().GetProperty("Result");
                            return property?.GetValue(result);
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        // 同步方法，直接返回结果
                        return result;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"调用方法 {functionName} 失败，错误：{ex.Message}");
                    return null;
                }
            }
            return null;
        }



        public async Task<object?> ExecuteFunctionsFromToolCallAsync(List<Delegate> delegates, string functionName, Dictionary<string, string> arguments)
        {

            //Debug.WriteLine(SerializeDelegates(delegates));

            foreach (var del in delegates)
            {
                var method = del.Method;

                // 检查方法名是否匹配
                if (method.GetCustomAttribute<FunctionDescriptionAttribute>()?.Name == functionName || method.Name == functionName)
                {
                    // 获取方法的参数信息
                    var parameters = method.GetParameters();
                    var parameterValues = new object[parameters.Length];

                    for (int i = 0; i < parameters.Length; i++)
                    {
                        var param = parameters[i];
                        if (arguments.TryGetValue(param.Name!, out var argValue))
                        {
                            if (!string.IsNullOrWhiteSpace(argValue))
                            {
                                try
                                {
                                    parameterValues[i] = Convert.ChangeType(argValue, param.ParameterType);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"参数 {param.Name} 转换失败：{ex.Message}");
                                    return null;
                                }
                            }
                            else
                            {
                                return $"{param.Name} 参数需指明!";
                            }
                        }
                        else
                        {
                            Console.WriteLine($"参数 {param.Name} 未提供。");
                            parameterValues[i] = GetDefault(param.ParameterType) ?? new object();
                        }
                    }

                    try
                    {
                        // 调用委托
                        var result = del.DynamicInvoke(parameterValues);

                        // 检查是否返回 Task
                        if (result is Task task)
                        {
                            await task.ConfigureAwait(false);

                            // 如果是 Task<T>，获取结果
                            if (result.GetType().IsGenericType)
                            {
                                var property = result.GetType().GetProperty("Result");
                                return property?.GetValue(result);
                            }
                            else
                            {
                                return null;
                            }
                        }
                        else
                        {
                            return result;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"调用方法 {functionName} 失败，错误：{ex.Message}");
                        return null;
                    }
                }


            }
            return null;
        }



        // 方法：获取类型的默认值
        private static object? GetDefault(Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }
    }






}




