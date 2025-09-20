using Domain.Events;

namespace Domain.Conversations;

public class Conversation : Entity
{
    public string Name { get; private set; }

    private readonly List<ConversationThread> _threads = [];

    public IReadOnlyCollection<ConversationThread> Threads => _threads;

    public Guid CurrentThread { get; private set; }

    public Guid UserId { get; private set; }

    public Conversation(Guid userId)
    {
        Id = Guid.NewGuid();
        Name = string.Empty;
        UserId = userId;
        CreateNewThread();
    }

    public Conversation(Guid id, Guid userId, string name, Guid currentThread, List<ConversationThread> threads)
    {
        Id = id;
        UserId = userId;
        Name = name;
        CurrentThread = currentThread;
        _threads = threads;
    }

    private void CreateNewThread()
    {
        var conversationThread = new ConversationThread();

        _threads.Add(conversationThread);

        CurrentThread = conversationThread.Id;
    }

    public void UpdateTitle(string title, Guid userId)
    {
        Name = title;

        AddDomainEvent(new ConversationTitleUpdatedEvent(userId,Id, Name));
    }

    public void AddUserMessage(UserMessage message)
    {
        ValidateAndAddMessageToThread(message);
    }

    public void AddAssistantMessage(AssistantMessage message, Guid userId)
    {
        ValidateAndAddMessageToThread(message);

        AddDomainEvent(new AssistantMessageAddedEvent(message, Id, userId));
    }

    private void ValidateAndAddMessageToThread(Message message)
    {
        Verify.NotNull(message);
        Verify.NotEmpty(CurrentThread);

        var thread = Threads.FirstOrDefault(x => x.Id == CurrentThread)
                     ?? throw new ArgumentException($"Thread {CurrentThread} does not exist in conversation {Id} ");

        thread.AddMessage(message);
    }
}