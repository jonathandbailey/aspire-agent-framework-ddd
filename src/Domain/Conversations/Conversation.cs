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

    public void StartConversationTurn(string content)
    {
        var thread = GetCurrentThread();

        thread.StartConversationTurn(content);
    }

    public void EndConversationTurn(string content)
    {
        var thread = GetCurrentThread();

        thread.EndConversationTurn(content);

        AddDomainEvent(new ConversationTurnEndedEvent(UserId, Id));
    }

    private void CreateNewThread()
    {
        var conversationThread = new ConversationThread(_threads.Count);

        _threads.Add(conversationThread);

        CurrentThread = conversationThread.Id;
    }

    public void UpdateTitle(string title)
    {
        Name = title;

        AddDomainEvent(new ConversationTitleUpdatedEvent(UserId, Id, Name));
    }

    private ConversationThread GetCurrentThread()
    {
        var thread = Threads.FirstOrDefault(x => x.Id == CurrentThread)
                     ?? throw new ArgumentException($"Thread {CurrentThread} does not exist in conversation {Id} ");

        return thread;
    }
}