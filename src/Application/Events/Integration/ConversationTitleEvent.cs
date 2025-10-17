using Domain.Conversations;
using MediatR;

namespace Application.Events.Integration;

public sealed record ConversationTitleEvent(Guid UserId, Guid ConversationId, List<Message> Messages) : IRequest;

public sealed record ConversationTitleUpdatedEvent(Guid UserId, Guid ConversationId, string Title) : IRequest;