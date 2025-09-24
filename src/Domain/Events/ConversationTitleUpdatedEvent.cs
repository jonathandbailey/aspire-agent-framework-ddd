using Domain.Conversations;
using MediatR;

namespace Domain.Events;

public sealed record ConversationTitleUpdatedEvent(UserId UserId, Guid ConversationId, string Title) : INotification;