using Domain.Conversations;
using MediatR;

namespace Application.Events.Integration;

public sealed record ConversationAgentEvent(UserId UserId, Guid ExchangeId, Guid ConversationId, List<Message> Messages)
    : IRequest; 