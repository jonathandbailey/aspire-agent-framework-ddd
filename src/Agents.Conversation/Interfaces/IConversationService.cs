namespace Agents.Conversation.Interfaces;

public interface IConversationService
{
    Task PublishDomainUpdate(Guid userId, string content, Guid conversationId, Guid exchangeId);
}