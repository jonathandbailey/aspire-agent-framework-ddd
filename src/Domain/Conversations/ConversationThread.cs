using Domain.Common;
using Domain.Exceptions;

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
        Verify.NotEmpty(id);
        Verify.NotNull(exchanges);
        
        Id = id;
        Index = index;
        _exchanges = exchanges;
    }

    public ExchangeId StartConversationExchange(string content, ExchangeId exchangeId)
    {
        var exchange = _exchanges.FirstOrDefault(x => Equals(x.ExchangeId, exchangeId));

        if (exchange == null)
            throw new ExchangeNotFoundDomainException($"Conversation Exchange {exchangeId.Value} does not exist.");

        exchange.UserMessage.Update(content);

        return exchange.ExchangeId;
    }

    public ExchangeId CreateConversationExchange()
    {
        var exchange = new ConversationExchange(ExchangeId.New(), _exchanges.Count, new UserMessage(string.Empty, 0), new AssistantMessage(string.Empty, 1));

        _exchanges.Add(exchange);

        return exchange.ExchangeId;
    }

    public void CompleteConversationExchange(ExchangeId exchangeId, string content)
    {
        var exchange = _exchanges.FirstOrDefault(x => Equals(x.ExchangeId, exchangeId));

        if (exchange == null)
            throw new ExchangeNotFoundDomainException($"Conversation Exchange {exchangeId.Value} does not exist.");

        exchange.AssistantMessage.Update(content);
    }
}