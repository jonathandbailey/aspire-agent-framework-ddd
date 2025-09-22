using Application.Dto;
using Domain.Conversations;

namespace Application.Interfaces;

public interface ITitleAssistant
{
    public Task<AssistantResponseDto> InvokeAsync(UserMessage userMessage);
}