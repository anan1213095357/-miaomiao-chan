using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Text;

namespace Blazor.Chat.Utility;

public class ProjectGenerator
{
    private readonly string _projectName;
    private readonly string _projectType;
    private readonly string _outputPath;

    public ProjectGenerator(string projectName, string projectType = "webapi", string? outputPath = null)
    {
        _projectName = projectName;
        _projectType = projectType;
        string projectPath = Path.Combine(AppContext.BaseDirectory, "Project");
        if (!Directory.Exists(projectPath))
            Directory.CreateDirectory(projectPath);
        _outputPath = outputPath ?? Path.Combine(projectPath!, projectName);
    }

    public async Task<bool> GenerateProjectAsync()
    {
        try
        {
            // 创建项目
            if (!await ExecuteCommandAsync($"dotnet new {_projectType} -n {_projectName}"))
                return false;

            // 创建 Docker 文件
            await GenerateDockerfileAsync();
            await GenerateWwwroot();

            // 创建 .dockerignore
            await GenerateDockerignoreAsync();



            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"生成项目时发生错误: {ex.Message}");
            return false;
        }
    }

    private async Task<bool> ExecuteCommandAsync(string command)
    {
        var processInfo = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            Arguments = $"/c {command}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            WorkingDirectory = Path.GetDirectoryName(_outputPath)
        };

        using var process = new Process { StartInfo = processInfo };
        process.OutputDataReceived += (sender, e) => { if (e.Data != null) Console.WriteLine(e.Data); };
        process.ErrorDataReceived += (sender, e) => { if (e.Data != null) Console.WriteLine(e.Data); };

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        await process.WaitForExitAsync();

        return process.ExitCode == 0;
    }


    private async Task GenerateWwwroot()
    {
        Directory.CreateDirectory(Path.Combine(_outputPath, "wwwroot"));
        File.Create(Path.Combine(_outputPath, "wwwroot", "index.html"));
        var program = $"var builder = WebApplication.CreateBuilder(args);\r\nbuilder.Services.AddOpenApi();\r\nvar app = builder.Build();\r\napp.MapOpenApi();\r\napp.UseStaticFiles();\r\napp.UseHttpsRedirection();\r\napp.Run();";
        await File.WriteAllTextAsync(Path.Combine(_outputPath, "Program.cs"), program);
    }


    private async Task GenerateDockerfileAsync()
    {
        // 将项目名变为全小写用于镜像名称以及中间路径处理
        var lowerProjectName = _projectName.ToLowerInvariant();
        var dockerfileContent = $@"FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY [""{_projectName}.csproj"", ""{lowerProjectName}/""]
WORKDIR ""/src/{lowerProjectName}""
RUN dotnet restore ""{_projectName}.csproj""
COPY . . 
RUN dotnet build ""{_projectName}.csproj"" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish ""{_projectName}.csproj"" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT [""dotnet"", ""{_projectName}.dll""]";

        var dockerfilePath = Path.Combine(_outputPath, "Dockerfile");
        await File.WriteAllTextAsync(dockerfilePath, dockerfileContent, Encoding.UTF8);
    }
    public void SetCustomPort(string filePath, int port)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("未找到 appsettings.json 文件。", filePath);
        }

        string jsonContent = File.ReadAllText(filePath);
        var jsonObject = JObject.Parse(jsonContent);

        // 准备新的Url
        string newUrl = $"http://*.:{port}";
        jsonObject["Urls"] = newUrl;

        File.WriteAllText(filePath, jsonObject.ToString());
    }

    private async Task GenerateDockerignoreAsync()
    {
        var dockerignoreContent = @"**/.classpath
**/.dockerignore
**/.env
**/.git
**/.gitignore
**/.project
**/.settings
**/.toolstarget
**/.vs
**/.vscode
**/*.*proj.user
**/*.dbmdl
**/*.jfm
**/azds.yaml
**/bin
**/charts
**/docker-compose*
**/Dockerfile*
**/node_modules
**/npm-debug.log
**/obj
**/secrets.dev.yaml
**/values.dev.yaml
LICENSE
README.md";

        var dockerignorePath = Path.Combine(_outputPath, ".dockerignore");
        await File.WriteAllTextAsync(dockerignorePath, dockerignoreContent, Encoding.UTF8);
    }
}
