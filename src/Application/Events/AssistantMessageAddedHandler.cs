using System.Text;
using Application.Extensions;
using Application.Interfaces;
using Domain.Conversations;
using Domain.Events;
using MediatR;

namespace Application.Events;

public class AssistantMessageAddedHandler(IConversationRepository conversationRepository,
    IMediator mediator,
    IAssistantFactory assistantFactory) : INotificationHandler<ConversationTurnEndedEvent>
{
    public async Task Handle(ConversationTurnEndedEvent request, CancellationToken cancellationToken)
    {
        var conversation = await conversationRepository.LoadAsync(request.UserId, request.ConversationId);

        if (string.IsNullOrWhiteSpace(conversation.Name))
        {
            var titleAssistant = await assistantFactory.CreateTitleAssistant();

            var threadSummary = ExtractThreadSummaryFromConversation(conversation);

            var userMessage = new UserMessage(threadSummary, 0);

            await foreach (var response in titleAssistant.StreamAsync(userMessage))
            {
                if (JsonOutputParser.HasJson(response))
                {
                    var conversationTitle = JsonOutputParser.Parse<ConversationTitle>(response);

                    conversation.UpdateTitle(conversationTitle.Title);
                }
            }

            await conversationRepository.SaveAsync(conversation);

            await mediator.PublishDomainEvents(conversation);
        }
    }

    private static string ExtractThreadSummaryFromConversation(Conversation conversation)
    {
        var thread = conversation.Threads.FirstOrDefault();

        if (thread == null)
        {
            throw new InvalidOperationException($"Conversation {conversation.Id} doesn't have any threads");
        }

        if (thread.Turns.Count == 0)
        {
            throw new InvalidOperationException($"Conversation Thread {thread.Id}, doesn't have any conversations turns. Conversation Id : {conversation.Id}");
        }

        var stringBuilder = new StringBuilder();

        foreach (var turn in thread.Turns)
        {
            stringBuilder.AppendLine(turn.UserMessage.Content);
            stringBuilder.AppendLine(turn.AssistantMessage.Content);
        }

        return stringBuilder.ToString();
    }
}