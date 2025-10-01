using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;

namespace Infrastructure.Agents.Interfaces;

public interface IBaseStreamingAgent
{
    IAsyncEnumerable<AgentResponseItem<StreamingChatMessageContent>> InvokeStreamAsync(
        ChatHistoryAgentThread thread);
}