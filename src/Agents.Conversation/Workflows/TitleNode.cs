using Agents.Conversation.State;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Agents.AI.Workflows.Reflection;
using System.Text;
using Agents.Conversation.Interfaces;

namespace Agents.Conversation.Workflows;

internal class TitleNode(AIAgent agent, IConversationService conversationService) : ReflectingExecutor<TitleNode>("TitleNode"), IMessageHandler<ConversationState, ConversationState>
{
    public async ValueTask<ConversationState> HandleAsync(ConversationState state, IWorkflowContext context,
        CancellationToken cancellationToken = new CancellationToken())
    {
        var stringBuilder = new StringBuilder();

        await foreach (var update in agent.RunStreamingAsync(state.Messages, cancellationToken: cancellationToken))
        {
            stringBuilder.Append(update.Text);
        }

        var title = stringBuilder.ToString();

        await conversationService.PublishConversationTitleStream(state.UserId, title, state.ConversationId);

        return state with { Response = title };
    }
}