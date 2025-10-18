using Domain.Conversations;
using MediatR;

namespace Domain.Events;

public sealed record ConversationExchangeCompletedEvent(
    UserId UserId,
    Guid ConversationId,
    Guid ThreadId,
    ExchangeId ExchangeId) : INotification;