using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Agents.AI.Workflows.Reflection;
using System.Text;
using Agents.Conversation.State;

namespace Agents.Conversation.Workflows;

public class ConversationNode(AIAgent agent) : ReflectingExecutor<ConversationNode>("ConversationNode"), IMessageHandler<ConversationState, ConversationState>
{
    public async ValueTask<ConversationState> HandleAsync(ConversationState state, IWorkflowContext context,
        CancellationToken cancellationToken = new CancellationToken())
    {
        var stringBuilder = new StringBuilder();

        await foreach (var update in agent.RunStreamingAsync(state.Messages, cancellationToken: cancellationToken))
        {
            stringBuilder.Append(update.Text);
            await context.AddEventAsync(new ConversationStreamingEvent(update.Text), cancellationToken);
        }

        return state with { Response = stringBuilder.ToString() };
    }
}

public class ConversationStreamingEvent(string message) : WorkflowEvent(message) { }