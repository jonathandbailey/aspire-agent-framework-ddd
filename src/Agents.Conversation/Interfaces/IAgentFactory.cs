
namespace Application.Interfaces;

public interface IAgentFactory
{
    Task<IAgent> CreateAgent(string name);
}