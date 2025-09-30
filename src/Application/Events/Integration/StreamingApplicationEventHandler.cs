using Application.Dto;
using Application.Interfaces;
using MediatR;

namespace Application.Events.Integration;

public class StreamingApplicationEventHandler(IMessageBus messageBus) : 
    IRequestHandler<UserStreamingApplicationEvent>,
    IRequestHandler<ConversationTitleUpdateApplicationEvent>
{
    public async Task Handle(UserStreamingApplicationEvent request, CancellationToken cancellationToken)
    {
        await messageBus.SendAsync(new ConversationStreamingMessage(request.UserId.Value, request.Content,
            request.ConversationId, request.ExchangeId), "UserConversationStream");
    }

    public async Task Handle(ConversationTitleUpdateApplicationEvent request, CancellationToken cancellationToken)
    {
        await messageBus.SendAsync(new ConversationTitleUpdatedMessage(request.UserId.Value, 
            request.ConversationId, request.Content), "ConversationTitleStream");
    }
}