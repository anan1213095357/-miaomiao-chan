using FreeSql.DataAnnotations;

namespace Blazor.Chat.Models
{
    [Table(Name = "chat_messages")]
    public class ChatMessage
    {
        [Column(IsPrimary = true, IsIdentity = true)]
        public long Id { get; set; }
        public string Guid { get; set; }

        public int Index { get; set; }

        // 将 ChatHistoryId 的类型更改为 long，与 ChatHistoryItem.Id 类型一致
        public long ChatHistoryId { get; set; }

        // 确保属性名称为 ChatHistory，类型为 ChatHistoryItem
        [Navigate(nameof(ChatHistoryId))]
        public ChatHistoryItem? ChatHistory { get; set; }

        public bool IsBot { get; set; }

        public string Content { get; set; } = string.Empty;

        public DateTime Timestamp { get; set; }
    }
}
