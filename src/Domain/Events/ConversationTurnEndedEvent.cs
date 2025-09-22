using MediatR;

namespace Domain.Events;

public sealed record ConversationTurnEndedEvent(Guid UserId, Guid ConversationId) : INotification;