using Domain.Common;

namespace Domain.Conversations;

public class UserMessage : Message
{
    public UserMessage(Guid id, int index, string content) : base(id, index, content, "user")
    {
    }

    public UserMessage(string content, int index) : base(Guid.NewGuid(), index, content, "user")
    {
    }
}