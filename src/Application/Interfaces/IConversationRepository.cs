using Domain.Conversations;

namespace Application.Interfaces;

public interface IConversationRepository
{
    Task SaveAsync(Conversation conversation);
    Task<Conversation> LoadAsync(Guid userId, Guid conversationId);
}