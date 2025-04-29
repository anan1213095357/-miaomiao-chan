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
            try
            {
                // 检查key.text文件是否存在
                string filePath = "key.txt";
                if (!File.Exists(filePath))
                {
                    // 如果文件不存在，返回空字符串
                    Console.WriteLine("未找到key.text文件");
                    return "";
                }

                // 读取文件内容
                string[] lines = File.ReadAllLines(filePath);

                // 查找对应服务的key
                foreach (string line in lines)
                {
                    // 解析每一行，格式应为 "服务名=API密钥"
                    string[] parts = line.Split('=', 2);
                    if (parts.Length == 2)
                    {
                        string service = parts[0].Trim();
                        string key = parts[1].Trim();

                        // 如果找到匹配的服务名，返回对应的key
                        if (service.Equals(settings.CurrentService, StringComparison.OrdinalIgnoreCase))
                        {
                            return key;
                        }
                    }
                }

                // 如果没有找到匹配的服务，返回空字符串
                return "";
            }
            catch (Exception ex)
            {
                // 异常处理
                Console.WriteLine($"读取API密钥时出错: {ex.Message}");
                return "";
            }
        }
    }
}