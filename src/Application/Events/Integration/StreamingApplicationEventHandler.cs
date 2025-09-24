using Application.Dto;
using Application.Interfaces;
using MediatR;

namespace Application.Events.Integration;

public class StreamingApplicationEventHandler(IConversationClient client) : IRequestHandler<StreamingApplicationEvent>
{
    public async Task Handle(StreamingApplicationEvent request, CancellationToken cancellationToken)
    {
        await client.ChatWithUser(request.UserId.Value, new ChatResponseDto(request.MessageId, request.Content, request.ConversationId));
    }
}