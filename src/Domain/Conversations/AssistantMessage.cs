namespace Domain.Conversations;

public class AssistantMessage(Guid id, string content) : Message(id, content, "assistant")
{
    public static AssistantMessage Create()
    {
        return new AssistantMessage(Guid.NewGuid(), string.Empty);
    }
}