using Domain.Common;

namespace Domain.Conversations;

public class AssistantMessage : Message
{
    public AssistantMessage(Guid id, int index, string content) : base(id,index, content, "assistant")
    {
    }

    public AssistantMessage(string content, int index) : base(Guid.NewGuid(), index, content, "assistant")
    {
    }
}