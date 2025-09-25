using Application.Interfaces;
using Domain.Conversations;
using MediatR;

namespace Application.Conversations.Commands;

public sealed record CreateConversationCommand(Guid UserId) : IRequest<Guid>;

public class CreateConversationCommandHandler(IConversationRepository conversationHistory) : IRequestHandler<CreateConversationCommand, Guid>
{
    public async Task<Guid> Handle(CreateConversationCommand request, CancellationToken cancellationToken)
    {
        var conversation = new Conversation(UserId.FromGuid(request.UserId));

        await conversationHistory.SaveAsync(conversation);

        return conversation.Id;
    }
}

