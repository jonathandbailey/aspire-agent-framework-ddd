using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace Agents.Infrastructure;

public class ChatHistoryContextProvider : AIContextProvider
{
    private readonly List<ChatMessage> _messages = [];
    
    public override async ValueTask<AIContext> InvokingAsync(InvokingContext context, CancellationToken cancellationToken = new CancellationToken())
    {
        return new AIContext { Messages  = _messages};
    }

    public void AddMessageAsync(ChatMessage message)
    {
        _messages.Add(message);
    }
}