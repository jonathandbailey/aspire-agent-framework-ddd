using Application.Dto;
using Application.Interfaces;
using MediatR;

namespace Application.Events.Integration;

public class ConversationTitleEventHandler(IMessageBus messageBus) : IRequestHandler<ConversationTitleEvent>
{
    public async Task Handle(ConversationTitleEvent request, CancellationToken cancellationToken)
    {
        await messageBus.SendAsyncToSummarize(new ConversationTitleMessage(request.UserId, request.ConversationId,
            request.Messages));
    }
}