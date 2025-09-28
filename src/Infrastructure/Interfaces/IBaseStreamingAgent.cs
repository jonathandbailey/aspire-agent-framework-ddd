using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;

namespace Infrastructure.Interfaces;

public interface IBaseStreamingAgent
{
    IAsyncEnumerable<AgentResponseItem<StreamingChatMessageContent>> InvokeStreamAsync(
        ChatHistoryAgentThread thread);
}