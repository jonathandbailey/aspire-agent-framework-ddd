using MediatR;

namespace Application.Events;

public sealed record StreamingApplicationEvent(Guid UserId, Guid MessageId , Guid ConversationId, string Content) : IRequest;