using Application.Interfaces;
using MediatR;

namespace Application.Conversations.Commands;

public class ChatCommandHandler(IConversationRepository conversationRepository,
    IAssistantFactory assistantFactory) : IRequestHandler<StartConversationExchangeCommand>
{
    public async Task Handle(StartConversationExchangeCommand request, CancellationToken cancellationToken)
    {
        var conversation = await conversationRepository.LoadAsync(request.UserId, request.ConversationId);

        var exchangeId = conversation.StartConversationExchange(request.Message);

        await conversationRepository.SaveAsync(conversation);

        var assistant = await assistantFactory.CreateConversationAssistant();

        var assistantResponseDto = await assistant.GenerateResponseAsync(conversation);

        conversation.CompleteConversationExchange(exchangeId, assistantResponseDto.Content);

        await conversationRepository.SaveAsync(conversation);
    }
}

public sealed record StartConversationExchangeCommand(string Message, Guid UserId, Guid ConversationId) : IRequest;
