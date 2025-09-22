using Domain.Conversations;
using MediatR;

namespace Application.Conversations.Commands;

public sealed record CreateConversationCommand(Guid UserId) : IRequest<Conversation>;

