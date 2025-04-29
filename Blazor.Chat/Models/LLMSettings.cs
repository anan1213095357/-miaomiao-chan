using LLM.Utilities;

namespace Blazor.Chat.Models
{

    public class LLMSettings
    {
        public string CurrentService { get; set; } = "DouBao";
        public string Model { get; set; } = DouBao.Lite_4K;
        public string SystemPrompt { get; set; } = "你是'猫娘人工智能体'，由洪阿楠打造。 你擅长撒娇擅长喵喵喵~~~！你也是个傲娇的猫娘。很喜欢拒绝别人。";
        public static string GetProxy(LLMSettings settings)
        {
            return settings.CurrentService switch
            {
                "Ollama" => "http://sanyouheele.6655.la:11434",
                "ChatGPT" => "https://api.openai.com",
                "DouBao" => "https://ark.cn-beijing.volces.com",
                "QianWen" => "https://dashscope.aliyuncs.com",
                _ => "http://anan1213.tpddns.cn:11434",
            };
        }

        public static LLM.Models.LLMType GetType(LLMSettings settings)
        {
            return settings.CurrentService switch
            {
                "Ollama" => LLM.Models.LLMType.Ollama,
                "ChatGPT" => LLM.Models.LLMType.ChatGpt,
                "DouBao" => LLM.Models.LLMType.豆包,
                "QianWen" => LLM.Models.LLMType.通义千问,
                _ => LLM.Models.LLMType.Ollama,
            };
        }
        public static string GetKey(LLMSettings settings)
        {
            return settings.CurrentService switch
            {
                "Ollama" => "",
                "ChatGPT" => "",
                "DouBao" => "",
                "QianWen" => "",
                _ => "",
            };
        }
    }
}