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

            var threadSummary = conversation.GetConversationSummaryForTitleGeneration();

            var userMessage = new UserMessage(threadSummary, 0);

            var response = await titleAssistant.InvokeAsync(userMessage);
          
            conversation.UpdateTitle(response.Content);

            await conversationRepository.SaveAsync(conversation);

            await mediator.PublishDomainEvents(conversation);
        }
    }
}