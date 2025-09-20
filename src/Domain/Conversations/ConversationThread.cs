namespace Domain.Conversations;

public class ConversationThread : Entity
{
    private readonly List<Message> _messages = [];

    public IReadOnlyCollection<Message> Messages => _messages;

    public ConversationThread()
    {
        Id = Guid.NewGuid();
    }

    public ConversationThread(Guid id, List<Message> messages)
    {
        Id = id;
        _messages = messages;
    }

    public void AddMessage(Message message)
    {
        _messages.Add(message);
    }
}