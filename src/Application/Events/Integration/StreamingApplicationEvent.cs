using Domain.Conversations;
using MediatR;

namespace Application.Events.Integration;

public sealed record StreamingApplicationEvent(UserId UserId, Guid ExchangeId , Guid ConversationId, string Content) : IRequest;