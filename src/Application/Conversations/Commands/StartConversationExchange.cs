using Application.Events.Integration;
using Application.Interfaces;
using Domain.Conversations;
using Domain.Interfaces;
using MediatR;

namespace Application.Conversations.Commands;

public class StartConversationExchangeCommandHandler(IConversationRepository conversationRepository, 
    IMediator mediator,
    IConversationDomainService conversationDomainService
    ) : IRequestHandler<StartConversationExchangeCommand>
{
    public async Task Handle(StartConversationExchangeCommand request, CancellationToken cancellationToken)
    {
        var conversation = await conversationRepository.LoadAsync(request.UserId, request.ConversationId);
            
        conversation.StartConversationExchange(request.Message, ExchangeId.FromGuid(request.ExchangeId));

        await conversationRepository.SaveAsync(conversation);
    
        var messages = conversationDomainService.GetMessages(conversation);

        await mediator.Send(new ConversationExchangeStartedIntegrationEvent(conversation.UserId, request.ExchangeId, conversation.Id
            , conversation.Name, messages), cancellationToken);
    }
}

public sealed record StartConversationExchangeCommand(string Message, Guid UserId, Guid ConversationId, Guid ExchangeId) : IRequest;
