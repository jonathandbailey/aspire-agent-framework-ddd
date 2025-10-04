
namespace Agents.Infrastructure.Interfaces;

public interface IAgentFactory
{
    Task<IAgent> CreateAgent(string templateName);
}