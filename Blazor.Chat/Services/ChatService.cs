using Blazor.Chat.Models;
using Blazor.Chat.Utility;
using LLM.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazor.Chat.Services
{
    public interface IChatService
    {
        event Action OnHistoryUpdate;

        Task SaveChatHistory(long userId, List<ChatMessage> messages, string? chatId = null);

        Task<List<ChatHistoryItem>> GetChatHistory(long userId);

        Task<ChatHistoryItem?> GetChatById(long userId, string chatId);

        Task DeleteChat(long userId, string chatId);
    }


    public class ChatService(
        ISettingsService settingsService,
        HttpClient httpClient,
        IFreeSql freeSql,
        IMiaoMiaoJiangAuthenticationStateProvider authProvider) : IChatService
    {
        private readonly ISettingsService _settingsService = settingsService;
        private readonly HttpClient _httpClient = httpClient;
        private readonly IFreeSql _freeSql = freeSql;
        private readonly IMiaoMiaoJiangAuthenticationStateProvider _authProvider = authProvider;

        public event Action? OnHistoryUpdate;

        public async Task SaveChatHistory(long userId, List<ChatMessage> messages, string? chatId = null)
        {
            if (messages == null || messages.Count == 0) return;

            var histories = await GetChatHistory(userId) ?? [];
            string summary = GenerateSummary(messages) ?? "";
            var history = histories.FirstOrDefault(p => p.ChatHistoryItemId == chatId);

            var repo = _freeSql.GetRepository<ChatHistoryItem>();
            repo.DbContextOptions.EnableCascadeSave = true;

            if (string.IsNullOrEmpty(chatId))
            {
                // 创建新的聊天历史
                var newHistory = new ChatHistoryItem
                {
                    UserId = userId,
                    DateTime = DateTime.Now,
                    Messages = messages,
                    LastMessage = GetLastMessage(messages),
                    Summary = summary
                };

                await repo.InsertAsync(newHistory);
            }
            else
            {
                // 更新现有的聊天历史
                if (history != null)
                {
                    history.Messages = messages;
                    history.LastMessage = messages.LastOrDefault()?.Content ?? "";
                    history.DateTime = DateTime.Now;
                    history.Summary = summary;

                    await repo.UpdateAsync(history); ;
                }
                else
                {
                    // 处理 chatId 不存在的情况（创建新的）
                    var newHistory = new ChatHistoryItem
                    {
                        ChatHistoryItemId = chatId,
                        UserId = userId,
                        DateTime = DateTime.Now,
                        Messages = messages,
                        LastMessage = messages.LastOrDefault()?.Content ?? "",
                        Summary = summary
                    };

                    await repo.InsertAsync(newHistory);
                }
            }

            // 保持最多50条聊天历史
            var userHistories = _freeSql.Select<ChatHistoryItem>()
                                        .Where(h => h.UserId == userId)
                                        .OrderByDescending(h => h.DateTime)
                                        .ToList();

            if (userHistories.Count > 50)
            {
                var historiesToDelete = userHistories.Skip(50).ToList();
                foreach (var hist in historiesToDelete)
                {
                    await DeleteChat(userId, hist.ChatHistoryItemId);
                }
            }

            OnHistoryUpdate?.Invoke();
        }

        public async Task<List<ChatHistoryItem>> GetChatHistory(long userId)
        {
            return await _freeSql.Select<ChatHistoryItem>()
                                .Where(h => h.UserId == userId)
                                .OrderByDescending(h => h.DateTime)
                                .IncludeMany(h => h.Messages) // 预加载消息
                                .ToListAsync();
        }

        public async Task<ChatHistoryItem?> GetChatById(long userId, string chatId)
        {
            return await _freeSql.Select<ChatHistoryItem>()
                                .Where(h => h.UserId == userId && h.ChatHistoryItemId == chatId)
                                .IncludeMany(h => h.Messages)
                                .FirstAsync();
        }


        public async Task DeleteChat(long userId, string chatId)
        {
            var chat = await _freeSql.Select<ChatHistoryItem>()
                                     .Where(h => h.UserId == userId && h.ChatHistoryItemId == chatId)
                                     .FirstAsync();
            if (chat != null)
            {
                await _freeSql.Delete<ChatHistoryItem>().Where(h => h.ChatHistoryItemId == chatId).ExecuteAffrowsAsync();
                OnHistoryUpdate?.Invoke();
            }
        }

        private string GenerateSummary(List<ChatMessage> messages)
        {
            var userMessage = messages.FirstOrDefault(p => !p.IsBot);
            if (userMessage?.Content is { } content)
            {
                // 安全截取：内容不足10字符时返回全文，否则取前10
                return content.Length > 10 ? content[..10] : content;
            }
            return "";
        }
        private string GetLastMessage(List<ChatMessage> messages)
        {
            var userMessage = messages.LastOrDefault(p => p.IsBot);
            if (userMessage?.Content is { } content)
            {
                // 安全截取：内容不足10字符时返回全文，否则取前10
                return content.Length > 20 ? content[..20] : content;
            }
            return "";
        }
    }
}
