namespace Domain.Conversations;

public class ConversationThread : Entity
{
    private readonly List<ConversationTurn> _turns = [];
    public int Index { get; private set; }

    public IReadOnlyCollection<ConversationTurn> Turns => _turns;

    public ConversationThread(int index)
    {
        Id = Guid.NewGuid();
        Index = index;
    }

    public ConversationThread(Guid id, int index, List<ConversationTurn> turns)
    {
        Id = id;
        Index = index;
        _turns = turns;
    }

    public void StartConversationTurn(string content)
    {
        _turns.Add(new ConversationTurn(Guid.NewGuid(), _turns.Count,  new UserMessage(content, 0), new AssistantMessage(string.Empty, 1)));
    }

    public void EndConversationTurn(string content)
    {
        var turn = _turns.Last();

        turn.AssistantMessage.Update(content);
    }
}