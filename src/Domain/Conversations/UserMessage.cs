namespace Domain.Conversations;

public class UserMessage : Message
{
    public UserMessage(Guid id, string content) : base(id, content, "user")
    {
    }

    public UserMessage(string content) : base(Guid.NewGuid(), content, "user")
    {
    }
}