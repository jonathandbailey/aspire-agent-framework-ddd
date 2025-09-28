using Application.Dto;
using Domain.Conversations;

namespace Application.Interfaces;

public interface IConversationAgent
{
    IAsyncEnumerable<AssistantResponseDto> InvokeStreamAsync(
        List<Message> messages);
}