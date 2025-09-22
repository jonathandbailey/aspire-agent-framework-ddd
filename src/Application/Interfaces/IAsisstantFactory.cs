
namespace Application.Interfaces;

public interface IAssistantFactory
{
    
    Task<IConversationAssistant> CreateConversationAssistant();

    Task<ITitleAssistant> CreateTitleAssistant();
}