
namespace Application.Interfaces;

public interface IAgentFactory
{
    Task<ITitleAssistant> CreateTitleAssistant();

    Task<IAgent> CreateAgent(string name);
}