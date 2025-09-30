using Domain.Conversations;
using MediatR;

namespace Application.Events.Integration;

public sealed record UserStreamingApplicationEvent(UserId UserId, Guid ExchangeId , Guid ConversationId, string Content) : IRequest;

public sealed record ConversationTitleUpdateApplicationEvent(UserId UserId, Guid ConversationId, string Content) : IRequest;