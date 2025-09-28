using Domain.Conversations;

namespace Domain.Interfaces;

public interface IConversationDomainService
{
    string GetConversationSummary(Conversation conversation);
    List<Message> GetMessages(Conversation conversation);
}