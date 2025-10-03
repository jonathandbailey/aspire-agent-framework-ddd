using Agents.Conversation.Dto;

namespace Agents.Conversation.Interfaces;

public interface IAgentService
{
    Task<AssistantResponseDto> ProcessAsync(ConversationAgentMessage conversation);
}