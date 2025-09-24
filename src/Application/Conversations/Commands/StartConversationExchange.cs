using Application.Interfaces;
using Domain.Conversations;
using MediatR;

namespace Application.Conversations.Commands;

public class StartConversationExchangeCommandHandler(IConversationRepository conversationRepository,
    IAssistantFactory assistantFactory) : IRequestHandler<StartConversationExchangeCommand>
{
    public async Task Handle(StartConversationExchangeCommand request, CancellationToken cancellationToken)
    {
        var conversation = await conversationRepository.LoadAsync(request.UserId, request.ConversationId);
            
        conversation.StartConversationExchange(request.Message, ExchangeId.FromGuid(request.ExchangeId));

        await conversationRepository.SaveAsync(conversation);

        var assistant = await assistantFactory.CreateConversationAssistant();

        var assistantResponseDto = await assistant.GenerateResponseAsync(conversation);

        conversation.CompleteConversationExchange(ExchangeId.FromGuid(request.ExchangeId), assistantResponseDto.Content);

        await conversationRepository.SaveAsync(conversation);
    }
}

public sealed record StartConversationExchangeCommand(string Message, Guid UserId, Guid ConversationId, Guid ExchangeId) : IRequest;
