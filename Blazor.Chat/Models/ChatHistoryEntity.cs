using FreeSql.DataAnnotations;

namespace Blazor.Chat.Models;

public class ChatHistoryEntity
{
    [Column(IsPrimary = true, IsIdentity = false)]
    public string Id { get; set; } = "";

    public string UserId { get; set; } = "";

    public DateTime DateTime { get; set; }

    public List<ChatMessage> Messages { get; set; } = new();

    public string LastMessage { get; set; } = "";

    public string Summary { get; set; } = "";
}