using Domain.Conversations;

namespace Application.Interfaces;

public interface IConversationAssistant
{
    Task<AssistantMessage>  GenerateResponseAsync(Conversation conversation);
}