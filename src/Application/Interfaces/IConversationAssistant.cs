using Domain.Conversations;

namespace Application.Interfaces;

public interface IConversationAssistant
{
    Task GenerateResponseAsync(Conversation conversation, Guid assistantMessageId);
}