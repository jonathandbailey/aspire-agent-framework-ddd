using Domain.Common;
using Domain.Events;
using System.Text;

namespace Domain.Conversations;

public class Conversation : Entity
{
    public string Name { get; private set; }

    private readonly List<ConversationThread> _threads = [];

    public IReadOnlyCollection<ConversationThread> Threads => _threads;
   
    public Guid UserId { get; }

    public Conversation(Guid userId)
    {
        Id = Guid.NewGuid();
        Name = string.Empty;
        UserId = userId;
        CreateNewThread();
    }

    public Conversation(Guid id, Guid userId, string name, List<ConversationThread> threads)
    {
        Id = id;
        UserId = userId;
        Name = name;
        _threads = threads;
    }

    public ExchangeId StartConversationExchange(string content)
    {
        var thread = GetCurrentThread();

        return thread.StartConversationExchange(content);
    }

    public void CompleteConversationExchange(ExchangeId exchangeId, string content)
    {
        var thread = GetCurrentThread();

        thread.CompleteConversationExchange(exchangeId, content);

        AddDomainEvent(new ConversationTurnEndedEvent(UserId, Id));
    }

    public string GetConversationSummaryForTitleGeneration()
    {
        var thread = Threads.FirstOrDefault();

        if (thread == null)
        {
            throw new InvalidOperationException($"Conversation {Id} doesn't have any threads");
        }

        if (thread.Exchanges.Count == 0)
        {
            throw new InvalidOperationException($"Conversation Thread {thread.Id}, doesn't have any conversations turns. Conversation Id : {Id}");
        }

        var stringBuilder = new StringBuilder();

        foreach (var turn in thread.Exchanges)
        {
            stringBuilder.AppendLine(turn.UserMessage.Content);
            stringBuilder.AppendLine(turn.AssistantMessage.Content);
        }

        return stringBuilder.ToString();
    }

    private void CreateNewThread()
    {
        var conversationThread = new ConversationThread(_threads.Count);

        _threads.Add(conversationThread);
    }

    public void UpdateTitle(string title)
    {
        Name = title;

        AddDomainEvent(new ConversationTitleUpdatedEvent(UserId, Id, Name));
    }

    private ConversationThread GetCurrentThread()
    {
        return Threads.Last();
    }
}