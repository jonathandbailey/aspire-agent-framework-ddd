using Domain.Conversations;

namespace Application.Interfaces;

public interface IConversationRepository
{
    Task SaveAsync(Guid conversationId, Conversation conversation);
    Task<Conversation> LoadAsync(Guid conversationId);
    Task<List<Conversation>> GetAllConversations();
}