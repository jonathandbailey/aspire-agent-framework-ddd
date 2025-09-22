using MediatR;

namespace Application.Events.Integration;

public sealed record StreamingApplicationEvent(Guid UserId, Guid MessageId , Guid ConversationId, string Content) : IRequest;