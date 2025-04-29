
using System.Text.Json;
using Blazor.Chat.Models;
using Blazor.Chat.Components.Pages;
using Microsoft.JSInterop;

namespace Blazor.Chat.Services;

public interface ISettingsService
{
    Task<LLMSettings> LoadSettingsAsync();
    Task SaveSettingsAsync(LLMSettings settings);
    Task<List<OllamaModel>> GetAvailableModels(string ollamaHost);
    Task InstallModel(string ollamaHost, string modelName);
    Task<bool> PingOllama(string ollamaHost);
    Task DeleteModel(string ollamaHost, string modelName);
}

public class SettingsService : ISettingsService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly IJSRuntime _jsRuntime;
    private readonly string _settingsPath;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly static string _settingsKey = "llm";

    public SettingsService(IHttpClientFactory httpClientFactory, IConfiguration configuration,IJSRuntime jsRuntime)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _jsRuntime = jsRuntime;
        _settingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.json");
        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };
    }

    private HttpClient CreateClient()
    {
        return _httpClientFactory.CreateClient("OllamaClient");
    }


    private async Task<LLMSettings> CreateDefaultSettingsAsync()
    {
        var settings = new LLMSettings();
        // 确保设置文件存在
        await SaveSettingsAsync(settings);
        return settings;
    }

    public async Task<LLMSettings> LoadSettingsAsync()
    {
        try
        {
            var json = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", _settingsKey);
            if (!string.IsNullOrEmpty(json))
            {
                return JsonSerializer.Deserialize<LLMSettings>(json, _jsonOptions) ?? new LLMSettings();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading settings: {ex.Message}");
        }

        return await CreateDefaultSettingsAsync();
    }

    public async Task SaveSettingsAsync(LLMSettings settings)
    {
        try
        {
            var json = JsonSerializer.Serialize(settings, _jsonOptions);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", _settingsKey, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving settings: {ex.Message}");
            throw;
        }
    }
    public async Task<bool> PingOllama(string ollamaHost)
    {
        try
        {
            using var client = CreateClient();
            var response = await client.GetAsync($"{ollamaHost}/api/tags");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<List<OllamaModel>> GetAvailableModels(string ollamaHost)
    {
        try
        {
            using var client = CreateClient();
            var response = await client.GetAsync($"{ollamaHost}/api/tags");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<OllamaTagsResponse>(content, _jsonOptions);

                var settings = await LoadSettingsAsync();

                return result?.Models.Select(m => new OllamaModel
                {
                    Name = m.Name,
                    Size = FormatSize(m.Size)
                }).ToList() ?? [];
            }

            return [];
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting available models: {ex.Message}");
            return [];
        }
    }

    public async Task InstallModel(string ollamaHost, string modelName)
    {
        try
        {
            using var client = CreateClient();
            var content = new StringContent(
                JsonSerializer.Serialize(new { model = modelName }, _jsonOptions),
                System.Text.Encoding.UTF8,
                "application/json");

            var response = await client.PostAsync($"{ollamaHost}/api/pull", content);
            response.EnsureSuccessStatusCode();

            // 更新已安装模型列表
            var settings = await LoadSettingsAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error installing model: {ex.Message}");
            throw;
        }
    }

    private string FormatSize(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        int order = 0;
        double size = bytes;

        while (size >= 1024 && order < sizes.Length - 1)
        {
            order++;
            size = size / 1024;
        }

        return $"{size:0.##} {sizes[order]}";
    }

    public async Task DeleteModel(string ollamaHost, string modelName)
    {
        try
        {
            using var client = CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Delete, $"{ollamaHost}/api/delete")
            {
                Content = new StringContent(
                    JsonSerializer.Serialize(new { model = modelName }, _jsonOptions),
                    System.Text.Encoding.UTF8,
                    "application/json")
            };

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            // 更新已安装模型列表
            var settings = await LoadSettingsAsync();
            if (settings.Model == modelName)
            {
                settings.Model = "";
                await SaveSettingsAsync(settings);
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting model: {ex.Message}");
            throw;
        }
    }

    // Ollama API 响应模型
    private class OllamaTagsResponse
    {
        public List<OllamaModelInfo> Models { get; set; } = new();
    }

    private class OllamaModelInfo
    {
        public string Name { get; set; } = string.Empty;
        public long Size { get; set; }
    }
}