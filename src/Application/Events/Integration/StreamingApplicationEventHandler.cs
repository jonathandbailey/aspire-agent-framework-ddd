using Application.Dto;
using Application.Interfaces;
using MediatR;

namespace Application.Events.Integration;

public class StreamingApplicationEventHandler(IConversationClient client, IMessageBus messageBus) : IRequestHandler<StreamingApplicationEvent>
{
    public async Task Handle(StreamingApplicationEvent request, CancellationToken cancellationToken)
    {
        await client.ChatWithUser(request.UserId.Value, new ChatResponseDto(request.ExchangeId, request.Content, request.ConversationId));

        await messageBus.PublishToUser(new ConversationStreamingMessage(request.UserId.Value, request.Content,
            request.ConversationId, request.ExchangeId));
    }
}