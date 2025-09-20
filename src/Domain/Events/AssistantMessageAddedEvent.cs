using Domain.Conversations;
using MediatR;

namespace Domain.Events;

public sealed record AssistantMessageAddedEvent(AssistantMessage Message, Guid ConversationId, Guid UserId) :INotification;