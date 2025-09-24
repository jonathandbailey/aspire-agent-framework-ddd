using Application.Interfaces;
using MediatR;

namespace Application.Conversations.Commands;

public class CreateConversationExchangeCommandHandler(IConversationRepository conversationRepository) : IRequestHandler<CreateConversationExchangeCommand, Guid>
{
    public async Task<Guid> Handle(CreateConversationExchangeCommand request, CancellationToken cancellationToken)
    {
        var conversation = await conversationRepository.LoadAsync(request.UserId, request.ConversationId);

        var exchangeId = conversation.CreateConversationExchange();

        await conversationRepository.SaveAsync(conversation);

        return exchangeId.Value;
    }
}

public sealed record CreateConversationExchangeCommand(Guid UserId, Guid ConversationId) : IRequest<Guid>;