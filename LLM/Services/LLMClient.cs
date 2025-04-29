using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using static LLM.Services.LLMClient;
using System.Threading;
using System.IO;
using LLM.Models;
using LLM.Utilities.Ollama;
using LLM.Models.ChatGpt;
using System.Net.Http.Headers;
using LLM.Models.Ollama;
using LLM.Utilities;
using LLM.Models.DouBao;
using System.Reflection;

namespace LLM.Services
{
    public class LLMClient : IDisposable
    {


        private readonly List<ToolBase> _tools = [];
        public ChatRequestBase ChatRequest { get; set; } = new();
        private readonly LLMOption lLMOption = new();
        private readonly HttpClient _httpClient = new();
        public Func<string, Task>? StreamDelegate;
        public Action<decimal>? StreamCompletedDelegate;


        public class LLMOption
        {
            public string BaseUrl { get; set; } = default!;
            public string Model { get; set; } = default!;
            public string Format { get; set; } = default!;
            public string? Bearer { get; set; }
            public Models.LLMType LLMType { get; set; }
            public object Options { get; set; } = default!;
            public object? KeepAlive { get; set; }
            public List<Type> FunctionTypes { get; set; } = [];
            public List<MessageBase> InitialMessages { get; set; } = [];

            public HttpClient HttpClient = new();
            public List<Delegate> FunctionDelegates { get; set; } = [];
        }







        public LLMClient(LLMOption lLMOption)
        {
            lLMOption.HttpClient.Timeout = TimeSpan.FromSeconds(300);
            switch (lLMOption.LLMType)
            {
                case Models.LLMType.Ollama:
                    {
                        _tools = [];
                        this.lLMOption = lLMOption;
                        _httpClient = lLMOption.HttpClient;
                        BuildTools();
                        var chatRequeset = new Models.Ollama.ChatRequest
                        {
                            Model = lLMOption.Model,
                            Stream = false,
                            Format = lLMOption.Format,
                            Options = lLMOption.Options,
                            KeepAlive = lLMOption.KeepAlive,
                            Tools = _tools.Select(p => (p as Models.Ollama.Tool) ?? new()).ToList(),
                            Messages = new List<Models.MessageBase>(lLMOption.InitialMessages).Select(p => (Models.Ollama.Message)p).ToList(),
                        };
                        ChatRequest = chatRequeset;
                    }
                    break;

                case Models.LLMType.豆包:
                    {
                        _tools = [];
                        this.lLMOption = lLMOption;
                        _httpClient = lLMOption.HttpClient;
                        BuildTools();
                        var chatRequeset = new Models.DouBao.ChatRequest
                        {
                            Model = lLMOption.Model,
                            Stream = false,
                            Format = lLMOption.Format,
                            Options = lLMOption.Options,
                            KeepAlive = lLMOption.KeepAlive,
                            Tools = _tools.Select(p => (p as Models.DouBao.Tool) ?? new()).ToList(),
                            Messages = new List<Models.MessageBase>(lLMOption.InitialMessages).Select(p => (Models.DouBao.Message)p).ToList(),
                        };
                        ChatRequest = chatRequeset;
                    }
                    break;
                case Models.LLMType.通义千问:
                    {
                        _tools = [];
                        this.lLMOption = lLMOption;
                        _httpClient = lLMOption.HttpClient;
                        BuildTools();
                        var chatRequeset = new Models.QianWen.ChatRequest
                        {
                            Model = lLMOption.Model,
                            Stream = false,
                            Tools = _tools.Select(p => (p as Models.QianWen.Tool) ?? new()).ToList(),
                            Messages = new List<Models.MessageBase>(lLMOption.InitialMessages).Select(p => (Models.QianWen.Message)p).ToList(),
                        };
                        ChatRequest = chatRequeset;
                    }
                    break;



                case Models.LLMType.ChatGpt:
                    {
                        this.lLMOption = lLMOption;
                        _httpClient = lLMOption.HttpClient;
                        BuildTools();

                        var chatRequeset = new Models.ChatGpt.ChatRequset
                        {
                            Model = lLMOption.Model,
                            Messages = new List<Models.MessageBase>(lLMOption.InitialMessages).Select(p => (Models.ChatGpt.Message)p).ToList(),
                        };

                        if (_tools.Count != 0)
                            chatRequeset.Tools = _tools.Select(p => (p as Models.ChatGpt.Tool) ?? new()).ToList();
                        ChatRequest = chatRequeset;

                    }
                    break;
                default:
                    break;
            }


        }

        private void BuildTools()
        {
            foreach (var type in lLMOption.FunctionTypes)
            {
                ReflectionHelper.AddMethodsAsTools(type, _tools, lLMOption.LLMType);
            }
            foreach (var @delegate in lLMOption.FunctionDelegates)
            {
                ReflectionHelper.AddMethodsAsTools(@delegate, _tools, lLMOption.LLMType);
            }
        }


        public async Task<Models.Ollama.EmbeddingsResponse?> GetOllamaEmbeddingsAsync(IEnumerable<string> input)
        {
            var json = JsonConvert.SerializeObject(new
            {
                model = lLMOption.Model,
                input = input.ToArray()
            });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(lLMOption.BaseUrl + "/api/embed", content);

            if (response.IsSuccessStatusCode)
            {
                var resultString = await response.Content.ReadAsStringAsync();

                var chatResponse = JsonConvert.DeserializeObject<Models.Ollama.EmbeddingsResponse>(resultString)!;
                return chatResponse;
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"请求失败：{response.StatusCode}, 详情：{error}");
            }
        }

        public async Task<Models.Ollama.EmbeddingsResponse?> GetOllamaEmbeddingsAsync(string input)
        {
            var json = JsonConvert.SerializeObject(new
            {
                model = lLMOption.Model,
                input
            });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(lLMOption.BaseUrl + "/api/embed", content);

            if (response.IsSuccessStatusCode)
            {
                var resultString = await response.Content.ReadAsStringAsync();

                var chatResponse = JsonConvert.DeserializeObject<Models.Ollama.EmbeddingsResponse>(resultString)!;
                return chatResponse;
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"请求失败：{response.StatusCode}, 详情：{error}");
            }
        }

        public string ExtractMessages(ChatResponseBase chatResponse)
        {
            if (chatResponse != null)
            {
                switch (lLMOption.LLMType)
                {
                    case LLMType.Ollama:
                        {
                            var res = chatResponse as Models.Ollama.Response;
                            return res!.Message!.Content!;
                        }
                    case LLMType.豆包:
                        {
                            var res = chatResponse as Models.DouBao.Response;
                            return res!.Choices?.FirstOrDefault()?.Message?.Content!;
                        }
                    case LLMType.通义千问:
                        {
                            var res = chatResponse as Models.QianWen.Response;
                            return res!.Choices?.FirstOrDefault()?.Message?.Content!;
                        }
                    case LLMType.ChatGpt:
                        {
                            var res = chatResponse as Models.ChatGpt.Response;
                            return res?.choices?.FirstOrDefault()?.message?.content ?? "";
                        }
                    default:
                        return "";
                }
            }
            return "";

        }


        public bool IsFunctionCall(ChatResponseBase chatResponse)
        {
            switch (lLMOption.LLMType)
            {
                case LLMType.Ollama:
                    {
                        var res = chatResponse as Models.Ollama.Response;
                        return res!.Message!.ToolCalls != null && res!.Message.ToolCalls.Count > 0;
                    }
                case LLMType.豆包:
                    {
                        var res = chatResponse as Models.DouBao.Response;
                        return res?.Choices?.FirstOrDefault()?.Message?.ToolCalls?.Length > 0;
                    }
                case LLMType.通义千问:
                    {
                        var res = chatResponse as Models.QianWen.Response;
                        return res?.Choices?.FirstOrDefault()?.Message?.ToolCalls?.Length > 0;
                    }
                case LLMType.ChatGpt:
                    {
                        var res = chatResponse as Models.ChatGpt.Response;
                        return res?.choices?.FirstOrDefault()?.message?.tool_calls?.Count > 0;
                    }
                default:
                    return false;
            }
        }


        public async Task<(ChatResponseBase, decimal)?> SendMessageAsync(string userMessage, bool? isStream, bool? isFuncCall)
        {
            switch (lLMOption.LLMType)
            {
                case LLMType.Ollama:
                    {
                        return await OllamaSendMessageAsync(userMessage, isStream, isFuncCall);
                    }
                case LLMType.豆包:
                    {
                        return await DoubaoSendMessageAsync(userMessage, isStream, isFuncCall);
                    }
                case LLMType.通义千问:
                    {
                        return await QianWenSendMessageAsync(userMessage, isStream, isFuncCall);
                    }
                case LLMType.ChatGpt:
                    {
                        return await ChatGptSendMessageAsync(userMessage, isStream, isFuncCall);
                    }
                default:
                    return null;
            }
        }


        public class ConversationMessage
        {
            public bool IsUser { get; set; }
            public string Message { get; set; }
        }


        public void AddConversationMessage(List<ConversationMessage> conversationMessages)
        {
            foreach (var item in conversationMessages)
            {
                switch (lLMOption.LLMType)
                {
                    case LLMType.Ollama:
                        {
                            var chatRequest = (Models.Ollama.ChatRequest)ChatRequest;
                            chatRequest.Messages = [];
                            chatRequest.Messages?.Add(new Models.Ollama.Message
                            {
                                Content = item.Message,
                                Role = item.IsUser ? "user" : "assistant",
                            });
                            break;
                        }
                    case LLMType.豆包:
                        {
                            var chatRequest = (Models.DouBao.ChatRequest)ChatRequest;
                            chatRequest.Messages = [];
                            chatRequest.Messages?.Add(new Models.DouBao.Message
                            {
                                Content = item.Message,
                                Role = item.IsUser ? "user" : "assistant",
                            });
                            break;
                        }
                    case LLMType.通义千问:
                        {
                            var chatRequest = (Models.QianWen.ChatRequest)ChatRequest;
                            chatRequest.Messages = [];
                            chatRequest.Messages?.Add(new Models.QianWen.Message
                            {
                                Content = item.Message,
                                Role = item.IsUser ? "user" : "assistant",
                            });
                            break;
                        }
                    case LLMType.ChatGpt:
                        {
                            var chatRequest = (Models.ChatGpt.ChatRequset)ChatRequest;
                            chatRequest.Messages = [];
                            chatRequest.Messages?.Add(new Models.ChatGpt.Message
                            {
                                content = item.Message,
                                role = item.IsUser ? "user" : "assistant",
                            });
                            break;
                        }
                }
            }



        }


        private async Task<(Models.Ollama.Response, decimal)?> OllamaSendMessageAsync(string userMessage, bool? isStream, bool? isFuncCall)
        {


            try
            {
                int prompt_tokens = 0, completion_tokens = 0;
                var chatRequest = (Models.Ollama.ChatRequest)ChatRequest;
                chatRequest.Stream = isStream ?? false;

                chatRequest.Messages!.Add(new Models.Ollama.Message { Role = "user", Content = userMessage });

                if (!isFuncCall ?? false)
                {
                    chatRequest.Tools = null;
                }

                var json = JsonConvert.SerializeObject(chatRequest);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(lLMOption.BaseUrl + "/api/chat", content);
                if (response.IsSuccessStatusCode)
                {
                    if (isStream ?? false)
                    {
                        using var stream = await response.Content.ReadAsStreamAsync();
                        using var reader = new System.IO.StreamReader(stream);
                        StringBuilder sb = new();
                        while (!reader.EndOfStream)
                        {
                            try
                            {
                                var line = await reader.ReadLineAsync();
                                if (!string.IsNullOrEmpty(line))
                                {
                                    var obj = JsonConvert.DeserializeObject<Models.Ollama.StreamResponse>(line);

                                    if (StreamDelegate != null)
                                        await StreamDelegate(obj?.message?.Content ?? "");

                                    sb.Append(obj?.message?.Content ?? "");
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }


                        }
                        chatRequest.Messages.Add(new Models.Ollama.Message { Role = "assistant", Content = sb.ToString() });
                        var consume = MultiAPIPriceCalculator.CalculateTotalCost(lLMOption.LLMType, chatRequest.Model!, prompt_tokens, completion_tokens);
                        StreamCompletedDelegate?.Invoke(consume);
                        return null;
                    }
                    else
                    {
                        var resultString = await response.Content.ReadAsStringAsync();

                        var chatResponse = JsonConvert.DeserializeObject<Models.Ollama.Response>(resultString)!;
                        chatRequest.Messages.Add(new Models.Ollama.Message { Role = chatResponse.Message!.Role, Content = chatResponse.Message.Content });
                        var consume = MultiAPIPriceCalculator.CalculateTotalCost(lLMOption.LLMType, chatRequest.Model!, prompt_tokens, completion_tokens);
                        return (chatResponse, consume);
                    }



                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(error);
                    return default;
                }
            }
            catch (Exception)
            {

                return default;
            }



        }

        private async Task<(Models.ChatGpt.Response, decimal)?> ChatGptSendMessageAsync(string userMessage, bool? isStream, bool? isFuncCall)
        {
            try
            {
                var chatRequest = (Models.ChatGpt.ChatRequset)ChatRequest;
                int prompt_tokens = 0, completion_tokens = 0;

                if (isStream ?? false)
                {
                    chatRequest.Stream = true;
                    chatRequest.StreamOptions = new();
                }
                else if (!isStream ?? false)
                {
                    chatRequest.Stream = false;
                    chatRequest.StreamOptions = null;
                }
                if (chatRequest.Model?.Contains("o1") ?? false)
                {

                    chatRequest.Messages.First(p => p.role == "system").role = "assistant";
                    if (!isFuncCall ?? false)
                    {
                        chatRequest.Tools = null;
                    }
                }
                if (!string.IsNullOrEmpty(lLMOption.Bearer))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", lLMOption.Bearer);
                }
                chatRequest.Messages?.Add(new Models.ChatGpt.Message { role = "user", content = userMessage });



                var json = JsonConvert.SerializeObject(chatRequest);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(lLMOption.BaseUrl + "/v1/chat/completions", content);

                if (response.IsSuccessStatusCode)
                {
                    if (isStream ?? false)
                    {
                        using var stream = await response.Content.ReadAsStreamAsync();
                        using var reader = new System.IO.StreamReader(stream);
                        StringBuilder sb = new();
                        while (!reader.EndOfStream)
                        {
                            try
                            {
                                var line = await reader.ReadLineAsync();
                                if (!string.IsNullOrEmpty(line))
                                {
                                    line = line.Replace("data: ", "");
                                    if (line.Contains("[DONE]"))
                                        break;

                                    var obj = JsonConvert.DeserializeObject<Models.ChatGpt.StreamResponse>(line);

                                    if (StreamDelegate != null)
                                        await StreamDelegate(obj?.Choices?.FirstOrDefault()?.delta?.content ?? "");

                                    sb.Append(line);

                                    if (obj?.Usage?.prompt_tokens > 0)
                                    {
                                        prompt_tokens = obj.Usage.prompt_tokens;
                                    }

                                    if (obj?.Usage?.completion_tokens > 0)
                                    {
                                        completion_tokens = obj.Usage.completion_tokens;
                                    }

                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }


                        }
                        chatRequest.Messages.Add(new Models.ChatGpt.Message { role = "assistant", content = sb.ToString() });
                        var consume = MultiAPIPriceCalculator.CalculateTotalCost(lLMOption.LLMType, chatRequest.Model!, prompt_tokens, completion_tokens);
                        StreamCompletedDelegate?.Invoke(consume);


                        return null;
                    }
                    else
                    {
                        var resultString = await response.Content.ReadAsStringAsync();

                        var chatResponse = JsonConvert.DeserializeObject<Models.ChatGpt.Response>(resultString)!;
                        chatRequest.Messages.Add(new Models.ChatGpt.Message { role = chatResponse.choices?.First().delta?.role ?? "", content = chatResponse.choices?.First().delta?.content ?? "" });

                        var consume = MultiAPIPriceCalculator.CalculateTotalCost(lLMOption.LLMType, chatRequest.Model!, chatResponse.usage!.prompt_tokens, chatResponse.usage!.completion_tokens);
                        return (chatResponse, consume);
                    }



                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(response.StatusCode + error);
                    return default;
                }
            }
            catch (Exception)
            {

                return default;
            }
        }





        private async Task<(Models.DouBao.Response, decimal)?> DoubaoSendMessageAsync(string userMessage, bool? isStream, bool? isFuncCall)
        {
            int prompt_tokens = 0, completion_tokens = 0;
            var chatRequest = (Models.DouBao.ChatRequest)ChatRequest;
            chatRequest.Stream = isStream ?? false;
            chatRequest.Messages!.Add(new Models.DouBao.Message { Role = "user", Content = userMessage });
            if (!isFuncCall ?? false)
            {
                chatRequest.Tools = null;
            }
            chatRequest.Messages = chatRequest.Messages.Where(p => !string.IsNullOrEmpty(p.Role)).ToList();
            if (chatRequest.Stream)
            {
                chatRequest.IncludeUsageClass = new();
            }
            if (!string.IsNullOrEmpty(lLMOption.Bearer))
            {
                // 清除之前的 Authorization 头（如果有的话）
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", lLMOption.Bearer);
            }


            var json = JsonConvert.SerializeObject(chatRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(lLMOption.BaseUrl + "/api/v3/chat/completions", content);

            if (response.IsSuccessStatusCode)
            {
                if (isStream ?? false)
                {
                    using var stream = await response.Content.ReadAsStreamAsync();
                    using var reader = new System.IO.StreamReader(stream);
                    StringBuilder sb = new();
                    while (!reader.EndOfStream)
                    {

                        var line = await reader.ReadLineAsync();
                        try
                        {


                            if (!string.IsNullOrEmpty(line))
                            {
                                line = line.Replace("data: ", "");

                                if (line.Contains("[DONE]"))
                                {
                                    break;
                                }
                                var obj = JsonConvert.DeserializeObject<Models.DouBao.StreamResponse>(line);

                                if (StreamDelegate != null)
                                    await StreamDelegate(obj?.Choices.FirstOrDefault()?.Delta?.Content ?? "");

                                sb.Append(obj?.Choices.FirstOrDefault()?.Delta?.Content ?? "");

                                if (obj?.Usage?.PromptTokens > 0)
                                {
                                    prompt_tokens = obj.Usage.PromptTokens;
                                }

                                if (obj?.Usage?.CompletionTokens > 0)
                                {
                                    completion_tokens = obj.Usage.CompletionTokens;
                                }

                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }


                    }
                    chatRequest.Messages.Add(new Models.DouBao.Message { Role = "assistant", Content = sb.ToString() });
                    var consume = MultiAPIPriceCalculator.CalculateTotalCost(lLMOption.LLMType, chatRequest.Model!, prompt_tokens, completion_tokens);
                    StreamCompletedDelegate?.Invoke(consume);
                    return null;
                }
                else
                {
                    var resultString = await response.Content.ReadAsStringAsync();

                    var chatResponse = JsonConvert.DeserializeObject<Models.DouBao.Response>(resultString)!;
                    chatRequest.Messages.Add(new Models.DouBao.Message { Role = chatResponse.Choices!.FirstOrDefault()?.Delta?.Role, Content = chatResponse.Choices!.FirstOrDefault()?.Delta?.Content });
                    var consume = MultiAPIPriceCalculator.CalculateTotalCost(lLMOption.LLMType, chatRequest.Model!, prompt_tokens, completion_tokens);
                    return (chatResponse, consume);
                }
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"请求失败：{response.StatusCode}, 详情：{error}");
            }
        }






        private async Task<(Models.QianWen.Response, decimal)?> QianWenSendMessageAsync(string userMessage, bool? isStream, bool? isFuncCall)
        {
            int prompt_tokens = 0, completion_tokens = 0;
            var chatRequest = (Models.QianWen.ChatRequest)ChatRequest;
            chatRequest.Stream = isStream ?? false;

            if (chatRequest.Stream)
            {
                chatRequest.IncludeUsageClass = new();
            }
            if (!isFuncCall ?? false)
            {
                chatRequest.Tools = null;
            }
            chatRequest.Messages!.Add(new Models.QianWen.Message { Role = "user", Content = userMessage });
            chatRequest.Messages = chatRequest.Messages.Where(p => !string.IsNullOrEmpty(p.Role)).ToList();

            if (!string.IsNullOrEmpty(lLMOption.Bearer))
            {
                // 清除之前的 Authorization 头（如果有的话）
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", lLMOption.Bearer);
            }


            var json = JsonConvert.SerializeObject(chatRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(lLMOption.BaseUrl + "/compatible-mode/v1/chat/completions", content);

            if (response.IsSuccessStatusCode)
            {
                if (isStream ?? false)
                {
                    using var stream = await response.Content.ReadAsStreamAsync();
                    using var reader = new System.IO.StreamReader(stream);
                    StringBuilder sb = new();
                    while (!reader.EndOfStream)
                    {
                        try
                        {
                            var line = await reader.ReadLineAsync();


                            if (!string.IsNullOrEmpty(line))
                            {
                                line = line.Replace("data: ", "");

                                if (line.Contains("[DONE]"))
                                {
                                    break;
                                }

                                var obj = JsonConvert.DeserializeObject<Models.QianWen.StreamResponse>(line);

                                if (StreamDelegate != null)
                                    await StreamDelegate(obj?.Choices.FirstOrDefault()?.Delta?.Content ?? "");

                                sb.Append(obj?.Choices.FirstOrDefault()?.Delta?.Content ?? "");

                                if (obj?.Usage?.PromptTokens > 0)
                                {
                                    prompt_tokens = obj.Usage.PromptTokens;
                                }

                                if (obj?.Usage?.CompletionTokens > 0)
                                {
                                    completion_tokens = obj.Usage.CompletionTokens;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }


                    }
                    chatRequest.Messages.Add(new Models.QianWen.Message { Role = "assistant", Content = sb.ToString() });
                    var consume = MultiAPIPriceCalculator.CalculateTotalCost(lLMOption.LLMType, chatRequest.Model!, prompt_tokens, completion_tokens);
                    StreamCompletedDelegate?.Invoke(consume);
                    return null;
                }
                else
                {
                    var resultString = await response.Content.ReadAsStringAsync();

                    var chatResponse = JsonConvert.DeserializeObject<Models.QianWen.Response>(resultString)!;
                    chatRequest.Messages.Add(new Models.QianWen.Message { Role = chatResponse.Choices!.FirstOrDefault()?.Delta?.Role, Content = chatResponse.Choices!.FirstOrDefault()?.Delta?.Content });
                    var consume = MultiAPIPriceCalculator.CalculateTotalCost(lLMOption.LLMType, chatRequest.Model!, prompt_tokens, completion_tokens);
                    return (chatResponse, consume);
                }
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                return default;
            }
        }


        public async Task<object?> ExecuteFunctionsAsync(ChatResponseBase? response)
        {
            switch (lLMOption.LLMType)
            {
                case LLMType.Ollama:
                    {
                        return await ExecuteFunctionsAsync(response as Models.Ollama.Response);
                    }
                case LLMType.豆包:
                    {
                        return await ExecuteFunctionsAsync(response as Models.DouBao.Response);
                    }
                case LLMType.ChatGpt:
                    {
                        return await ExecuteFunctionsAsync(response as Models.ChatGpt.Response);
                    }
                case LLMType.通义千问:
                    {
                        return await ExecuteFunctionsAsync(response as Models.QianWen.Response);
                    }
                default:
                    return null;
            }
        }

        public async Task<object?> ExecuteFunctionsAsync(Models.Ollama.Response? response)
        {
            object? value = null;
            if (response?.Message?.ToolCalls != null && response.Message.ToolCalls.Count > 0)
            {
                foreach (var toolCall in response.Message.ToolCalls)
                {
                    var v1 = await new ReflectionHelper().ExecuteFunctionsFromToolCallAsync(lLMOption.FunctionTypes, toolCall.Function!.Name!, toolCall.Function.Arguments!);
                    if (v1 != null)
                    {
                        value += v1.ToString();
                        value += "\r\n";
                    }

                    var v2 = await new ReflectionHelper().ExecuteFunctionsFromToolCallAsync(lLMOption.FunctionDelegates, toolCall.Function.Name!, toolCall.Function.Arguments!);
                    if (v2 != null)
                    {
                        value += v2.ToString();
                        value += "\r\n";
                    }
                }

            }
            return value;
        }


        public async Task<object?> ExecuteFunctionsAsync(Models.ChatGpt.Response? response)
        {
            object? value = null;
            if (response?.choices?.FirstOrDefault()?.message?.tool_calls != null && response?.choices?.FirstOrDefault()?.message?.tool_calls?.Count > 0)
            {
                foreach (var toolCall in response?.choices?.FirstOrDefault()?.message?.tool_calls!)
                {
                    Dictionary<string, string> argumentsDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(toolCall.function!.arguments!) ?? []; ;

                    var v1 = await new ReflectionHelper().ExecuteFunctionsFromToolCallAsync(lLMOption.FunctionTypes, toolCall.function!.name!, argumentsDict);
                    if (v1 != null)
                    {
                        value += v1.ToString();
                        value += "\r\n";
                    }

                    var v2 = await new ReflectionHelper().ExecuteFunctionsFromToolCallAsync(lLMOption.FunctionDelegates, toolCall.function!.name!, argumentsDict);
                    if (v2 != null)
                    {
                        value += v2.ToString();
                        value += "\r\n";
                    }
                }

            }
            return value;
        }
        public async Task<object?> ExecuteFunctionsAsync(Models.DouBao.Response? response)
        {



            object? value = null;

            var choice = response?.Choices?.FirstOrDefault();

            if (choice != null && choice.Message?.ToolCalls != null)
            {
                foreach (var tool_call in choice.Message.ToolCalls)
                {
                    var argumentsDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(tool_call.Function?.Arguments ?? "");

                    var v1 = await new ReflectionHelper().ExecuteFunctionsFromToolCallAsync(lLMOption.FunctionTypes, tool_call.Function?.Name!, argumentsDict ?? []);
                    if (v1 != null)
                    {
                        value += v1.ToString();
                        value += "\r\n";
                    }

                    var v2 = await new ReflectionHelper().ExecuteFunctionsFromToolCallAsync(lLMOption.FunctionDelegates, tool_call.Function?.Name!, argumentsDict ?? []);
                    if (v2 != null)
                    {
                        value += v2.ToString();
                        value += "\r\n";
                    }


                }
            }

            //if (response?.Message.ToolCalls != null && response.Message.ToolCalls.Count > 0)
            //{
            //    foreach (var toolCall in response.Message.ToolCalls)
            //    {
            //        var v1 = ReflectionHelper.ExecuteFunctionsFromToolCall(_functionTypes, toolCall);
            //        if (v1 != null)
            //        {
            //            value += v1.ToString();
            //            value += "\r\n";
            //        }

            //        var v2 = ReflectionHelper.ExecuteFunctionsFromToolCall(_functionDelegates, toolCall);
            //        if (v2 != null)
            //        {
            //            value += v2.ToString();
            //            value += "\r\n";
            //        }
            //    }

            //}
            return value;
        }

        public async Task<object?> ExecuteFunctionsAsync(Models.QianWen.Response? response)
        {



            object? value = null;

            var choice = response?.Choices?.FirstOrDefault();

            if (choice != null && choice.Message?.ToolCalls != null)
            {
                foreach (var tool_call in choice.Message.ToolCalls)
                {
                    var argumentsDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(tool_call.Function?.Arguments ?? "");

                    var v1 = await new ReflectionHelper().ExecuteFunctionsFromToolCallAsync(lLMOption.FunctionTypes, tool_call.Function?.Name!, argumentsDict ?? []);
                    if (v1 != null)
                    {
                        value += v1.ToString();
                        value += "\r\n";
                    }

                    var v2 = await new ReflectionHelper().ExecuteFunctionsFromToolCallAsync(lLMOption.FunctionDelegates, tool_call.Function?.Name!, argumentsDict ?? []);
                    if (v2 != null)
                    {
                        value += v2.ToString();
                        value += "\r\n";
                    }


                }
            }
            return value;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            StreamDelegate = null;
            StreamCompletedDelegate = null;
        }
    }
}
