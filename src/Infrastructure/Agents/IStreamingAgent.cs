using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;

namespace Infrastructure.Agents;

public interface IStreamingAgent
{
    IAsyncEnumerable<AgentResponseItem<StreamingChatMessageContent>> InvokeStreamAsync(
        ChatHistoryAgentThread thread);
}