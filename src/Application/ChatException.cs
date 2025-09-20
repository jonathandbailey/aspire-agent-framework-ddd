namespace Application;

public class ChatException : Exception
{
    public Guid UserId { get; }
    public Guid ConversationId { get; }

    public ChatException(string message, Guid userId, Guid conversationId, Exception innerException)
        : base(message, innerException)
    {
        UserId = userId;
        ConversationId = conversationId;
    }
}