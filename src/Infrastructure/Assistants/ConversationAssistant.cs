using System.Text;
using Application.Dto;
using Application.Events.Integration;
using Application.Interfaces;
using Infrastructure.Interfaces;
using Microsoft.SemanticKernel.Agents;
using Conversation = Domain.Conversations.Conversation;


namespace Infrastructure.Assistants;

public class ConversationAssistant(ChatCompletionAgent chatCompletionAgent, IAssistantMemory memory, IStreamingEventPublisher publisher) : IConversationAssistant
{
    public async Task<AssistantResponseDto> GenerateResponseAsync(Conversation conversation, Guid exchangeId)
    {
        var thread = memory.Initialize(conversation);

        var stringBuilder = new StringBuilder();
        
        await foreach (var response in chatCompletionAgent.InvokeStreamingAsync(thread))
        {
            if (response.Message.Content != null)
            {
                await publisher.Send(new StreamingApplicationEvent(conversation.UserId, exchangeId, conversation.Id, response.Message.Content));

                stringBuilder.Append(response.Message.Content);
            }
        }

        return new AssistantResponseDto {Content = stringBuilder.ToString()};
    }
}