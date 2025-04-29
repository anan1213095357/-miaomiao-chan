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
                "ChatGPT" => "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",
                "DouBao" => "5f087934-8711-47ca-85bb-71e137db768a",
                "QianWen" => "sk-b5035b22b1d24f8ab4018cf8dfa0314e",
                _ => "",
            };
        }
    }
}