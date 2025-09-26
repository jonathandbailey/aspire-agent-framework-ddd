using Application.Dto;

namespace Application.Interfaces;

public interface IConversationQueries
{
    Task<List<Conversation>> GetAllConversationsAsync(Guid userId);
    Task<Conversation> LoadAsync(Guid userId, Guid conversationId);
    Task<List<ConversationSummaryItem>> GetConversationSummaries(Guid userId);
}