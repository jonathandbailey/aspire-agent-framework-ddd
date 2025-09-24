using Domain.Conversations;
using MediatR;

namespace Domain.Events;

public sealed record ConversationTurnEndedEvent(UserId UserId, Guid ConversationId) : INotification;