
namespace Application.Interfaces;

public interface IAssistantFactory
{
    Task<ITitleAssistant> CreateTitleAssistant();

    Task<IAgent> CreateConversationAgent();
}