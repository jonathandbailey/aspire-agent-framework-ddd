using Application.Interfaces;
using Domain.Conversations;
using MediatR;

namespace Application.Events.Integration;

public sealed record ConversationExchangeCompletedEvent(Guid UserId, string Content, Guid ConversationId, Guid ExchangeId) : IRequest;

public class ConversationExchangeCompletedEventHandler(IConversationRepository conversationRepository) : IRequestHandler<ConversationExchangeCompletedEvent>
{
    public async Task Handle(ConversationExchangeCompletedEvent request, CancellationToken cancellationToken)
    {
        var conversation = await conversationRepository.LoadAsync(request.UserId, request.ConversationId);

        conversation.CompleteConversationExchange(ExchangeId.FromGuid(request.ExchangeId), request.Content);

        await conversationRepository.SaveAsync(conversation);
    }
}