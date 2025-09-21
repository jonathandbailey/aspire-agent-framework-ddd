using Application.Dto;
using Conversation = Domain.Conversations.Conversation;

namespace Application.Interfaces;

public interface IConversationAssistant
{
    Task<AssistantResponseDto> GenerateResponseAsync(Conversation conversation, Guid assistantMessageId);
}