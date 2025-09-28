using Application.Dto;
using Domain.Conversations;

namespace Application.Interfaces;

public interface IAgent
{
    IAsyncEnumerable<AssistantResponseDto> InvokeStreamAsync(
        List<Message> messages);
}