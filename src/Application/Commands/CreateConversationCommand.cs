using Domain.Conversations;
using MediatR;

namespace Application.Commands;

public sealed record CreateConversationCommand(Guid UserId) : IRequest<Conversation>;

