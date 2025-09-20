using Domain.Conversations;

namespace Application.Interfaces;

public interface IChatService
{
    Task<AssistantMessage> Chat(UserMessage userMessage, Guid userId, Guid conversationId);
}