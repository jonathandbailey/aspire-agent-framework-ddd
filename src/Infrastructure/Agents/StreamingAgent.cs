using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;

namespace Infrastructure.Agents
{
    public class StreamingAgent(ChatCompletionAgent chatCompletionAgent) : IStreamingAgent
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
