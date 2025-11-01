using Microsoft.Agents.AI;

namespace Agents.Infrastructure.Interfaces;

public interface IAgentFactory
{
    Task<AIAgent> CreateAgent(Guid id);
}