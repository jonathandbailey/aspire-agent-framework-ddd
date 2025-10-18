using Agents.Infrastructure.Dto;
using Azure;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Agents.AI.Workflows.Reflection;
using System.Text;
using Microsoft.Extensions.AI;

namespace Agents.Conversation.Workflows;

public class ConversationNode(AIAgent agent) : ReflectingExecutor<ConversationNode>("ConversationNode"), IMessageHandler<List<ChatMessage>, string>
{
    public async ValueTask<string> HandleAsync(List<ChatMessage> messages, IWorkflowContext context,
        CancellationToken cancellationToken = new CancellationToken())
    {
        var stringBuilder = new StringBuilder();

        await foreach (var update in agent.RunStreamingAsync(messages, cancellationToken: cancellationToken))
        {
            stringBuilder.Append(update.Text);

            await context.AddEventAsync(new ConversationStreamingEvent(update.Text), cancellationToken);
        }

        return stringBuilder.ToString();
    }
}

public class ConversationStreamingEvent(string message) : WorkflowEvent(message) { }