using Agents.Conversation.Dto;
using Agents.Conversation.Extensions;
using Application.Interfaces;
using Infrastructure.Agents.Interfaces;

namespace Agents.Conversation;

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