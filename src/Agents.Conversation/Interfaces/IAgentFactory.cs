using Microsoft.Agents.AI;

namespace Agents.Infrastructure.Interfaces;

public interface IAgentFactory
{
    Task<AIAgent> CreateConversationAgent();
    Task<AIAgent> CreateTitleAgent();
    Task<AIAgent> CreateConversationReasonAgent();
}