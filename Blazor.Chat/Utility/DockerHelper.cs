using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Blazor.Chat.Utility;

public static class DockerHelper
{
    /// <summary>
    /// 执行 Docker build 命令
    /// </summary>
    /// <param name="imageName">镜像名称标签，例如 "test"</param>
    /// <param name="dockerfilePath">Dockerfile 的相对路径，例如 "test/Dockerfile"</param>
    /// <param name="buildContext">构建上下文路径，例如 "."</param>
    /// <returns></returns>
    public static async Task<(bool, string)> BuildImageAsync(string imageName, string dockerfilePath)
    {
        string arguments = $"build -t {imageName.ToLower()} -f {dockerfilePath}/Dockerfile {dockerfilePath}";
        Console.WriteLine($"Executing: docker {arguments}");
        var result = await ExecuteCommandAsync("docker", arguments);

        if (result.ExitCode == 0)
        {
            Console.WriteLine("Docker build succeeded.");
            return (true, result.Output);
        }
        else
        {
            Console.WriteLine($"Docker build failed with exit code {result.ExitCode}.");
            Console.WriteLine($"Error Output: {result.Error}");
            return (true, result.Output);
        }
    }

    /// <summary>
    /// 执行 Docker run 命令
    /// </summary>
    /// <param name="containerName">容器名称，例如 "my-test-app"</param>
    /// <param name="imageName">镜像名称，例如 "test"</param>
    /// <returns></returns>
    public static async Task<(bool, string)> RunContainerAsync(string containerName,int port)
    {
        string arguments = $"run -itd --name {containerName.ToLower()}  -p {port}:{port}  {containerName.ToLower()}";
        Console.WriteLine($"Executing: docker {arguments}");

        var result = await ExecuteCommandAsync("docker", arguments);

        if (result.ExitCode == 0)
        {
            Console.WriteLine("Docker run succeeded.");
            return (true, result.Output);
        }
        else
        {
            Console.WriteLine($"Docker run failed with exit code {result.ExitCode}.");
            Console.WriteLine($"Error Output: {result.Error}");
            return (false, result.Output);
        }
    }

    /// <summary>
    /// 通用的命令执行方法
    /// </summary>
    /// <param name="fileName">可执行文件名，例如 "docker"</param>
    /// <param name="arguments">命令行参数</param>
    /// <returns>包含标准输出、错误输出和退出代码的结果</returns>
    //private static async Task<(string Output, string Error, int ExitCode)> ExecuteCommandAsync(string fileName, string arguments)
    //{
    //    var tcs = new TaskCompletionSource<(string, string, int)>();


    //    var process = new Process
    //    {
    //        StartInfo = new ProcessStartInfo
    //        {
    //            FileName = fileName,
    //            Arguments = arguments,
    //            RedirectStandardOutput = true,
    //            RedirectStandardError = true,
    //            UseShellExecute = false,
    //            CreateNoWindow = true,
    //        },
    //        EnableRaisingEvents = true
    //    };

    //    string output = string.Empty;
    //    string error = string.Empty;

    //    process.OutputDataReceived += (sender, e) =>
    //    {
    //        if (e.Data != null)
    //        {
    //            output += e.Data + Environment.NewLine;
    //        }
    //    };

    //    process.ErrorDataReceived += (sender, e) =>
    //    {
    //        if (e.Data != null)
    //        {
    //            error += e.Data + Environment.NewLine;
    //        }
    //    };

    //    process.Exited += (sender, e) =>
    //    {
    //        tcs.SetResult((output, error, process.ExitCode));
    //        process.Dispose();
    //    };

    //    try
    //    {
    //        process.Start();
    //    }
    //    catch (Exception ex)
    //    {
    //        return (string.Empty, ex.Message, -1);
    //    }

    //    process.BeginOutputReadLine();
    //    process.BeginErrorReadLine();

    //    return await tcs.Task;
    //}

    private static async Task<(string Output, string Error, int ExitCode)> ExecuteCommandAsync(string fileName, string arguments)
    {
        var tcs = new TaskCompletionSource<(string, string, int)>();

        var processStartInfo = new ProcessStartInfo
        {
            FileName = fileName,
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,  // 不使用ShellExecute，适用于Linux和Windows
            CreateNoWindow = true
        };

        // Linux系统可能需要设置Shell环境变量，如bash等
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            processStartInfo.FileName = "/bin/bash";
            processStartInfo.Arguments = $"-c \"{fileName} {arguments}\"";  // 使用bash -c执行命令
        }

        var process = new Process
        {
            StartInfo = processStartInfo,
            EnableRaisingEvents = true
        };

        string output = string.Empty;
        string error = string.Empty;

        process.OutputDataReceived += (sender, e) =>
        {
            if (e.Data != null)
            {
                output += e.Data + Environment.NewLine;
            }
        };

        process.ErrorDataReceived += (sender, e) =>
        {
            if (e.Data != null)
            {
                error += e.Data + Environment.NewLine;
            }
        };

        process.Exited += (sender, e) =>
        {
            tcs.SetResult((output, error, process.ExitCode));
            process.Dispose();
        };

        try
        {
            process.Start();
        }
        catch (Exception ex)
        {
            return (string.Empty, ex.Message, -1);
        }

        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        return await tcs.Task;
    }

}
