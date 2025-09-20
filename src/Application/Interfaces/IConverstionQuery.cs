using Application.Dto;

namespace Application.Interfaces;

public interface IConversationQuery
{
    Task<List<Conversation>> GetAllConversationsAsync();
    Task<Conversation> LoadAsync(Guid conversationId);
}