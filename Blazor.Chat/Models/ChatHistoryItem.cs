using FreeSql.DataAnnotations;
using System.Collections.Generic;

namespace Blazor.Chat.Models
{
    [Table(Name = "chat_histories")]
    public class ChatHistoryItem
    {
        [Column(IsPrimary = true, IsIdentity = true)]
        public long Id { get; set; }  // 主键，类型为 long

        // 如果需要唯一标识符，可以保留，但不作为主键
        public string ChatHistoryItemId { get; set; } = Guid.NewGuid().ToString();

        public long UserId { get; set; }

        public DateTime DateTime { get; set; }

        // 一对多关系，指向 ChatMessage.ChatHistoryId 属性
        [Navigate(nameof(ChatMessage.ChatHistoryId))]
        public List<ChatMessage> Messages { get; set; } = [];

        public string LastMessage { get; set; } = string.Empty;

        public string Summary { get; set; } = string.Empty;
    }
}
