using Domain.Common;

namespace Domain.Conversations;

public class ConversationExchange : Entity
{
    public int Index { get; private set; }

    public ExchangeId ExchangeId { get; private set; }

    public UserMessage UserMessage { get; private set; }

    public AssistantMessage AssistantMessage { get; private set; }

    public ConversationExchange(Guid id, int index, UserMessage userMessage, AssistantMessage assistantMessage)
    {
        Id = id;
        ExchangeId = ExchangeId.FromGuid(id);
        Index = index;
        UserMessage = userMessage;
        AssistantMessage = assistantMessage;
    }
}