using Application.Dto;
using Application.Interfaces;
using MediatR;

namespace Application.Events.Integration;

public class ConversationAgentEventHandler(IMessageBus messageBus) : IRequestHandler<ConversationAgentEvent>
{
    public async Task Handle(ConversationAgentEvent request, CancellationToken cancellationToken)
    {
        await messageBus.SendAsync(new ConversationAgentMessage(request.UserId.Value, request.ExchangeId, request.ConversationId, request.Messages));
    }
}