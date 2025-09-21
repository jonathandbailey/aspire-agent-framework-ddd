namespace Domain.Conversations;

public class ConversationThread : Entity
{
    private readonly List<Message> _messages = [];

    public int Index { get; private set; }

    public IReadOnlyCollection<Message> Messages => _messages;

    public ConversationThread(int index)
    {
        Id = Guid.NewGuid();
        Index = index;
    }

    public ConversationThread(Guid id, int index, List<Message> messages)
    {
        Id = id;
        _messages = messages;
        Index = index;
    }

    public void AddMessage(Message message)
    {
        _messages.Add(message);
    }

    public void AddUserMessage(string content)
    {
        Verify.NotNullOrWhiteSpace(content);

        _messages.Add(new UserMessage(content, _messages.Count));
    }

    public AssistantMessage AddAssistantMessage(string content)
    {
        var message = new AssistantMessage(content, _messages.Count);


        _messages.Add(message);

        return message;
    }

    public Message UpdateAssistantMessage(Guid messageId, string content)
    {
        var message = _messages.FirstOrDefault(x => x.Id == messageId);

        if (message == null)
            throw new Exception($"Conversation Thread ({Id} does not contain a message with id ({messageId}");

        message.Update(content);

        return message;
    }
}