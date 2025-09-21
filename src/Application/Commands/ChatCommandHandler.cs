using Application.Extensions;
using Application.Interfaces;
using Domain;
using Domain.Conversations;
using MediatR;

namespace Application.Commands;

public class ChatCommandHandler(IConversationRepository conversationRepository,
    IMediator mediator,
    IAssistantFactory assistantFactory) : IRequestHandler<ChatCommand, AssistantMessage>
{
    public async Task<AssistantMessage> Handle(ChatCommand request, CancellationToken cancellationToken)
    {
        return await Chat( request.Id, request.Message, request.UserId, request.ConversationId);
    }

    private async Task<AssistantMessage> Chat(Guid id, string message, Guid userId, Guid conversationId)
    {
        Verify.NotEmpty(userId);
        Verify.NotEmpty(conversationId);
       
        var conversation = await conversationRepository.LoadAsync(userId, conversationId);

        conversation.AddUserMessage(message);

        var assistantMessage = conversation.AddAssistantMessage();

        await conversationRepository.SaveAsync(conversation);

        await mediator.PublishDomainEvents(conversation);

        var assistant = await assistantFactory.CreateConversationAssistant();

        await assistant.GenerateResponseAsync(conversation, assistantMessage.Id);

        await conversationRepository.SaveAsync(conversation);

        await mediator.PublishDomainEvents(conversation);

        return assistantMessage;
    }
}