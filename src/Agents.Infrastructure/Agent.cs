using Agents.Conversation.Extensions;
using Agents.Infrastructure.Dto;
using Agents.Infrastructure.Interfaces;
using Microsoft.Agents.AI;

namespace Agents.Infrastructure;

public class Agent(AIAgent agent) : IAgent
{
    public async IAsyncEnumerable<AssistantResponseDto> InvokeStreamAsync(List<Message> messages)
    {
        var thread = agent.GetNewThread();

        thread = thread.ToAgentThread(messages);
   
        await foreach (var response in agent.RunStreamingAsync(thread))
        {
            yield return new AssistantResponseDto { Content = response.Text };
        }
    }
}