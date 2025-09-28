using Domain.Common;

namespace Domain.Conversations;

public class Message(Guid id, int index, string content, string role)
    : BaseMessage(id, index, content, role);