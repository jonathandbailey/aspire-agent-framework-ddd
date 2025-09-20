
namespace Application.Interfaces;

public interface IAssistantFactory
{
    
    Task<IConversationAssistant> CreateConversationAssistant();

    Task<IAssistant> CreateTitleAssistant();
}