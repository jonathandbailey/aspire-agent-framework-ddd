using Application.Interfaces;
using Domain.Common;
using MediatR;

namespace Application.Conversations.Commands;

public class ChatCommandHandler(IConversationRepository conversationRepository,
    IAssistantFactory assistantFactory) : IRequestHandler<ChatCommand>
{
    public async Task Handle(ChatCommand request, CancellationToken cancellationToken)
    {
        await Chat(request.Message, request.UserId, request.ConversationId);
    }

    private async Task Chat(string message, Guid userId, Guid conversationId)
    {
        Verify.NotEmpty(userId);
        Verify.NotEmpty(conversationId);
       
        var conversation = await conversationRepository.LoadAsync(userId, conversationId);

        var exchangeId = conversation.StartConversationExchange(message);

        await conversationRepository.SaveAsync(conversation);
     
        var assistant = await assistantFactory.CreateConversationAssistant();

        var assistantResponseDto = await assistant.GenerateResponseAsync(conversation);

        conversation.CompleteConversationExchange(exchangeId, assistantResponseDto.Content);
  
        await conversationRepository.SaveAsync(conversation);
    }
}