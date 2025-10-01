using Agents.Conversation.Dto;

namespace Application.Interfaces;

public interface IAgent
{
    IAsyncEnumerable<AssistantResponseDto> InvokeStreamAsync(
        List<Message> messages);
}