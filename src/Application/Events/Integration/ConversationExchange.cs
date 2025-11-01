using Application.Dto;
using Application.Interfaces;
using Domain.Conversations;
using MediatR;

namespace Application.Events.Integration;

public sealed record ConversationExchangeCompletedIntegrationEvent(Guid UserId, string Content, Guid ConversationId, Guid ExchangeId) : IRequest;

public class ConversationExchangeCompletedEventHandler(IConversationRepository conversationRepository) : IRequestHandler<ConversationExchangeCompletedIntegrationEvent>
{
    public async Task Handle(ConversationExchangeCompletedIntegrationEvent request, CancellationToken cancellationToken)
    {
        var conversation = await conversationRepository.LoadAsync(request.UserId, request.ConversationId);

        conversation.CompleteConversationExchange(ExchangeId.FromGuid(request.ExchangeId), request.Content);

        await conversationRepository.SaveAsync(conversation);
    }
}

public sealed record ConversationExchangeStartedIntegrationEvent(UserId UserId, Guid ExchangeId, Guid ConversationId, string Title, List<Message> Messages)
    : IRequest;

public class ConversationExchangeStartedIntegrationEventHandler(IIntegrationMessaging messaging) : IRequestHandler<ConversationExchangeStartedIntegrationEvent>
{
    public async Task Handle(ConversationExchangeStartedIntegrationEvent request, CancellationToken cancellationToken)
    {
        await messaging.SendAsync(new ConversationExchangeStartedMessage(request.UserId.Value, request.ExchangeId, request.ConversationId, request.Title, request.Messages));
    }
}