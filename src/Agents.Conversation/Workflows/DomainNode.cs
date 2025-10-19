using Agents.Conversation.Interfaces;
using Agents.Conversation.State;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Agents.AI.Workflows.Reflection;

namespace Agents.Conversation.Workflows;

public class ConversationDomainNode(IConversationService conversationService) : ReflectingExecutor<ConversationDomainNode>("DomainNode"), IMessageHandler<ConversationState>
{
    public async ValueTask HandleAsync(ConversationState request, IWorkflowContext context,
        CancellationToken cancellationToken = new CancellationToken())
    {
        await conversationService.PublishDomainUpdate(request.UserId, request.Response,
            request.ConversationId, request.ExchangeId);
    }
}