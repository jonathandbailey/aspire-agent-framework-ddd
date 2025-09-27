using Domain.Common;
using Domain.Events;

namespace Domain.Conversations;

public class Conversation : Entity
{
    public string Name { get; private set; }

    private readonly List<ConversationThread> _threads = [];

    private readonly ConversationThread _activeThread;

    public IReadOnlyCollection<ConversationThread> Threads => _threads;
   
    public UserId UserId { get; }

    public Conversation(UserId userId)
    {
        Id = userId.Value;
        Name = string.Empty;
        UserId = userId;
        _activeThread = CreateNewThread();
    }

    public Conversation(Guid id, UserId userId, string name, List<ConversationThread> threads)
    {
        Verify.NotNull(id);
        Verify.NotNull(name);
        Verify.NotNull(threads);
        
        Id = id;
        UserId = userId;
        Name = name;
        _threads = threads;

        if (threads.Count == 0)
            throw new ArgumentException("threads collection must contain at least one ConversationThread.");

        _activeThread = threads.Last();
    }

    public ExchangeId CreateConversationExchange()
    {
        return _activeThread.CreateConversationExchange();
    }

    public void StartConversationExchange(string content, ExchangeId exchangeId)
    {
        _activeThread.StartConversationExchange(content, exchangeId);
    }

    public void CompleteConversationExchange(ExchangeId exchangeId, string content)
    {
        _activeThread.CompleteConversationExchange(exchangeId, content);

        AddDomainEvent(new ConversationTurnEndedEvent(UserId, Id));
    }

    private ConversationThread CreateNewThread()
    {
        var conversationThread = new ConversationThread(_threads.Count);
   
        _threads.Add(conversationThread);

        return conversationThread;
    }

    public void UpdateTitle(string title)
    {
        Name = title;

        AddDomainEvent(new ConversationTitleUpdatedEvent(UserId, Id, Name));
    }
}