using Application.Interfaces;
using MediatR;
using Domain.Conversations;

namespace Application.Conversations.Commands;

public class CreateConversationCommandHandler(IConversationRepository conversationHistory) : IRequestHandler<CreateConversationCommand, Conversation>
{
    public async Task<Conversation> Handle(CreateConversationCommand request, CancellationToken cancellationToken)
    {
        var conversation = new Conversation(request.UserId);

        await conversationHistory.SaveAsync(conversation);

        return conversation;
    }
}