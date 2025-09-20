using System.Text;
using Application.Events;
using Application.Interfaces;
using Domain.Conversations;
using Infrastructure.Interfaces;
using Microsoft.SemanticKernel.Agents;


namespace Infrastructure.Assistants;

public class ConversationAssistant(ChatCompletionAgent chatCompletionAgent, IAssistantMemory memory, IStreamingEventPublisher publisher) : IConversationAssistant
{
    public async Task<AssistantMessage> GenerateResponseAsync(Conversation conversation)
    {
        var thread = memory.Initialize(conversation);

        var stringBuilder = new StringBuilder();
        
        await foreach (var response in chatCompletionAgent.InvokeStreamingAsync(thread))
        {
            if (response.Message.Content != null)
            {
                await publisher.Send(new StreamingApplicationEvent(conversation.UserId, Guid.NewGuid(), conversation.Id, response.Message.Content));

                stringBuilder.Append(response.Message.Content);
            }
        }

        return new AssistantMessage(Guid.NewGuid(), stringBuilder.ToString());
    }
}