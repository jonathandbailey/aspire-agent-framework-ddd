using MediatR;

namespace Domain.Events;

public sealed record ConversationTitleUpdatedEvent(Guid UserId, Guid ConversationId, string Title) : INotification;