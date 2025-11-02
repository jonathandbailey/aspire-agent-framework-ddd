using Agents.Conversation.State;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Agents.AI.Workflows.Reflection;

namespace Agents.Conversation.Workflows.ReAct;

public class ConversationReasonNode(AIAgent agent) : ReflectingExecutor<ConversationNode>("ConversationReasonNode"), IMessageHandler<ConversationState, ConversationState>
{
    public async ValueTask<ConversationState> HandleAsync(ConversationState state, IWorkflowContext context,
        CancellationToken cancellationToken = new CancellationToken())
    {
        var response = await agent.RunAsync(state.Messages, cancellationToken: cancellationToken);
  
        return state with { Response = response.Text};
    }
}