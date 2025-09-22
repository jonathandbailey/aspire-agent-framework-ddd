namespace Domain.Conversations;

public class ConversationTurn : Entity
{
    public int Index { get; private set; }
    public UserMessage UserMessage { get; private set; }

    public AssistantMessage AssistantMessage { get; private set; }

    public ConversationTurn(Guid id, int index, UserMessage userMessage, AssistantMessage assistantMessage)
    {
        Id = id;
        Index = index;
        UserMessage = userMessage;
        AssistantMessage = assistantMessage;
    }
}