using Application.Commands;
using Application.Dto;
using Domain.Conversations;

namespace Application.Extensions;

public static class MappingExtensions
{
    public static ChatResponseDto MapToChatResponseDto(this AssistantMessage assistantMessage, Guid conversationId)
    {
        return new ChatResponseDto(assistantMessage.Id, assistantMessage.Content, conversationId);
    }

    public static ChatCommand Map(this ChatRequestDto requestDto, Guid userId)
    {
        return new ChatCommand(requestDto.Id, requestDto.Message, userId, requestDto.ConversationId);
    }
}