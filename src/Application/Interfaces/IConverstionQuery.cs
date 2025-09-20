using Application.Dto;

namespace Application.Interfaces;

public interface IConversationQuery
{
    Task<List<Conversation>> GetAllConversationsAsync(Guid userId);
    Task<Conversation> LoadAsync(Guid userId, Guid conversationId);
}