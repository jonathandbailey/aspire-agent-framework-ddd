using Application.Events.Integration;
using Application.Interfaces;
using Domain.Events;
using Domain.Interfaces;
using MediatR;
using ConversationExchangeCompletedEvent = Domain.Events.ConversationExchangeCompletedEvent;

namespace Application.Conversations.Events;

public class ConversationExchangeCompletedHandler(IConversationRepository conversationRepository, IConversationDomainService conversationDomainService, IMediator mediator) : INotificationHandler<ConversationExchangeCompletedEvent>
{
    public async Task Handle(ConversationExchangeCompletedEvent request, CancellationToken cancellationToken)
    {
        var conversation = await conversationRepository.LoadAsync(request.UserId.Value, request.ConversationId);

        if(!conversation.IsFirstExchange(request.ExchangeId, request.ThreadId))
            return;

        var messages = conversationDomainService.GetMessages(conversation);

        await mediator.Send(new ConversationTitleEvent(conversation.UserId.Value, conversation.Id, messages), cancellationToken);
    }
}