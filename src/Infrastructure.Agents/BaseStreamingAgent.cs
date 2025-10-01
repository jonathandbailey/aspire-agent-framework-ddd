using Infrastructure.Agents.Interfaces;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;

namespace Infrastructure.Agents
{
    public class BaseStreamingAgent(ChatCompletionAgent chatCompletionAgent) : IBaseStreamingAgent
    {
        public async IAsyncEnumerable<AgentResponseItem<StreamingChatMessageContent>> InvokeStreamAsync(
            ChatHistoryAgentThread thread)
        {
            await foreach (var response in chatCompletionAgent.InvokeStreamingAsync(thread))
            {
                yield return response;
            }
        }
    }
}
