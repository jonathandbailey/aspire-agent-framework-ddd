using Domain.Conversations;
using Microsoft.SemanticKernel.Agents;

namespace Infrastructure.Interfaces;

public interface IAssistantMemory
{
    ChatHistoryAgentThread Initialize(Conversation conversation);
}