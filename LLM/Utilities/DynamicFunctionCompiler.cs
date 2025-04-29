namespace LLM.Utilities;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using LLM.Attributes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

public class DynamicFunctionCompiler
{
    private List<MetadataReference> _references;

    public DynamicFunctionCompiler()
    {
        // 初始化程序集引用
        _references = new List<MetadataReference>(GetMetadataReferences());


    }

    /// <summary>
    /// 自动获取所有已加载程序集的 MetadataReference
    /// </summary>
    /// <returns>程序集引用列表</returns>
    private IEnumerable<MetadataReference> GetMetadataReferences()
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.Location))
            .Select(a => MetadataReference.CreateFromFile(a.Location));
    }

    // 使用线程安全的字典作为缓存
    private static readonly ConcurrentDictionary<string, Assembly> _assemblyCache = new ConcurrentDictionary<string, Assembly>();

    public (List<string>, Assembly?) CompileFunction(string functionCode)
    {
        // 计算代码的哈希值作为缓存键
        string codeHash = ComputeHash(functionCode);

        // 检查缓存中是否已经存在编译后的程序集
        if (_assemblyCache.TryGetValue(codeHash, out Assembly? cachedAssembly))
        {
            // 如果存在，直接返回缓存中的程序集
            return (new List<string>(), cachedAssembly);
        }

        // 解析语法树
        var syntaxTree = CSharpSyntaxTree.ParseText(functionCode);

        // 定义程序集名称
        string assemblyName = Path.GetRandomFileName();

        // 动态加载所有非动态程序集作为引用
        var references = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
            .Select(a => MetadataReference.CreateFromFile(a.Location))
            .Cast<MetadataReference>()
            .ToList();

        // 创建编译选项
        var compilation = CSharpCompilation.Create(
            assemblyName: assemblyName,
            syntaxTrees: new[] { syntaxTree },
            references: references,
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        using var ms = new MemoryStream();
        // 编译
        var result = compilation.Emit(ms);

        if (!result.Success)
        {
            // 处理编译错误
            var failures = result.Diagnostics.Where(diagnostic =>
                diagnostic.IsWarningAsError ||
                diagnostic.Severity == DiagnosticSeverity.Error);

            // 输出详细的错误信息，包括行号和列号
            var errors = failures.Select(f =>
            {
                var location = f.Location.GetLineSpan();
                var line = location.StartLinePosition.Line + 1; // 行号是从 0 开始的，输出时加 1
                var column = location.StartLinePosition.Character + 1; // 列号也是从 0 开始，输出时加 1
                return $"{f.Id}: {f.GetMessage()} at line {line}, column {column}";
            }).ToList();

            return (errors, null);
        }

        // 加载程序集
        ms.Seek(0, SeekOrigin.Begin);
        var assemblyBytes = ms.ToArray();
        var assemblyLoaded = Assembly.Load(assemblyBytes);

        // 将编译后的程序集添加到缓存
        _assemblyCache.TryAdd(codeHash, assemblyLoaded);

        return (new List<string>(), assemblyLoaded);
    }

    // 计算字符串的 SHA256 哈希值
    private string ComputeHash(string input)
    {
        byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        StringBuilder builder = new();
        foreach (var b in bytes)
            builder.Append(b.ToString("x2"));
        return builder.ToString();
    }

    /// <summary>
    /// 获取具有 FunctionDescriptionAttribute 的方法信息。
    /// </summary>
    /// <param name="assembly">已编译的程序集。</param>
    /// <returns>包含方法信息的列表。</returns>
    public List<MethodInfo> GetFunctionMethods(Assembly assembly)
    {
        var type = assembly.GetType("DynamicFunctions");
        if (type == null)
            throw new InvalidOperationException("未找到类型 'DynamicFunctions'。");

        // 获取带有 FunctionDescriptionAttribute 的方法
        var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                          .Where(m => m.GetCustomAttribute<FunctionDescriptionAttribute>() != null)
                          .ToList();

        return methods;
    }
}
