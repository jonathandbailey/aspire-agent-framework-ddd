using Domain.Common;

namespace Domain.Conversations;

public class ConversationThread : Entity
{
    private readonly List<ConversationExchange> _exchanges = [];
    public int Index { get; private set; }

    public IReadOnlyCollection<ConversationExchange> Exchanges => _exchanges;

    public ConversationThread(int index)
    {
        Id = Guid.NewGuid();
        Index = index;
    }

    public ConversationThread(Guid id, int index, List<ConversationExchange> exchanges)
    {
        Id = id;
        Index = index;
        _exchanges = exchanges;
    }

    public ExchangeId StartConversationExchange(string content)
    {
        var exchange = new ConversationExchange(ExchangeId.New(), _exchanges.Count, new UserMessage(content, 0), new AssistantMessage(string.Empty, 1));
  
        _exchanges.Add(exchange);

        return exchange.ExchangeId;
    }

    public void CompleteConversationExchange(ExchangeId exchangeId, string content)
    {
        var exchange = _exchanges.First(x => Equals(x.ExchangeId, exchangeId));

        exchange.AssistantMessage.Update(content);
    }
}