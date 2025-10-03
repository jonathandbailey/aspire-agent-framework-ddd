using Agents.Infrastructure.Dto;

namespace Agents.Infrastructure.Interfaces;

public interface IAgent
{
    IAsyncEnumerable<AssistantResponseDto> InvokeStreamAsync(
        List<Message> messages);
}