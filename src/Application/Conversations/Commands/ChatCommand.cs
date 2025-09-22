using Domain.Conversations;
using MediatR;

namespace Application.Conversations.Commands;

public sealed record ChatCommand(Guid Id, string Message, Guid UserId, Guid ConversationId) : IRequest;