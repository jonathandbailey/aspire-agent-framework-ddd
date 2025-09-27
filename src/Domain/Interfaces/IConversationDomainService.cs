using Domain.Conversations;

namespace Domain.Interfaces;

public interface IConversationDomainService
{
    string GetConversationSummary(Conversation conversation);
}