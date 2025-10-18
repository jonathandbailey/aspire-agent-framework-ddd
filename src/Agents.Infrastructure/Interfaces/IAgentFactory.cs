
using Microsoft.Agents.AI;

namespace Agents.Infrastructure.Interfaces;

public interface IAgentFactory
{
    Task<AIAgent> CreateAgent(string templateName);
    Task<IAgent> CreateWrappedAgent(string templateName);
}