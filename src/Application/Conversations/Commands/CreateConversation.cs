using Application.Interfaces;
using Domain.Conversations;
using MediatR;

namespace Application.Conversations.Commands;

public sealed record CreateConversationCommand(Guid UserId) : IRequest<Conversation>;

public class CreateConversationCommandHandler(IConversationRepository conversationHistory) : IRequestHandler<CreateConversationCommand, Conversation>
{
    public async Task<Conversation> Handle(CreateConversationCommand request, CancellationToken cancellationToken)
    {
        var conversation = new Conversation(UserId.FromGuid(request.UserId));

        await conversationHistory.SaveAsync(conversation);

        return conversation;
    }
}

