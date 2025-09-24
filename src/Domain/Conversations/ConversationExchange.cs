using Domain.Common;

namespace Domain.Conversations;

public class ConversationExchange : Entity
{
    public int Index { get; private set; }

    public ExchangeId ExchangeId { get; private set; }

    public UserMessage UserMessage { get; private set; }

    public AssistantMessage AssistantMessage { get; private set; }

    public ConversationExchange(ExchangeId exchangeId, int index, UserMessage userMessage, AssistantMessage assistantMessage)
    {
        Id = exchangeId.Value;
        ExchangeId = exchangeId;
        Index = index;
        UserMessage = userMessage;
        AssistantMessage = assistantMessage;
    }
}