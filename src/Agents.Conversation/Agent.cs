using Agents.Conversation.Dto;
using Agents.Conversation.Extensions;
using Application.Interfaces;
using Microsoft.Agents.AI;

namespace Agents.Conversation;

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