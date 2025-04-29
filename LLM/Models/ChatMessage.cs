namespace LLM.Models
{
    public class ChatMessage
    {
        public int Index { get; set; }
        public string? Content { get; set; }
        public bool IsBot { get; set; }
    }
}