using Application.Dto;
using Application.Interfaces;
using Domain.Conversations;
using Infrastructure.Extensions;
using Infrastructure.Interfaces;

namespace Infrastructure.Agents;

public class Agent(IBaseStreamingAgent agent) : IAgent
{
    public async IAsyncEnumerable<AssistantResponseDto> InvokeStreamAsync(
        List<Message> messages)
    {
        var thread = messages.ToChatHistoryThread();
        
        await foreach (var response in agent.InvokeStreamAsync(thread))
        {
            yield return new AssistantResponseDto { Content = response.Message.Content!};
        }
    }
}