using Application.Extensions;
using Application.Interfaces;
using Domain;
using Domain.Conversations;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Commands;

public class ChatCommandHandler(IConversationRepository conversationRepository,
    IMediator mediator,
    IAssistantFactory assistantFactory,
    ILogger<ChatCommandHandler> logger) : IRequestHandler<ChatCommand, AssistantMessage>
{
    public async Task<AssistantMessage> Handle(ChatCommand request, CancellationToken cancellationToken)
    {
        return await Chat( request.Id, request.Message, request.UserId, request.ConversationId);
    }

    private async Task<AssistantMessage> Chat(Guid id, string message, Guid userId, Guid conversationId)
    {
        Verify.NotEmpty(userId);
        Verify.NotEmpty(conversationId);

        try
        {
            var conversation = await conversationRepository.LoadAsync(conversationId);

            conversation.AddUserMessage(new UserMessage(id, message));

            var assistant = await assistantFactory.CreateConversationAssistant();

            var assistantMessage = await assistant.GenerateResponseAsync(conversation);
      
            conversation.AddAssistantMessage(assistantMessage, userId);

            await conversationRepository.SaveAsync(conversationId, conversation);

            await mediator.PublishDomainEvents(conversation);

            return assistantMessage;
        }
        catch (Exception exception)
        {
            var chatException = new ChatException("An exception has occurred while trying to execute chat", userId, conversationId, exception);

            logger.LogError(chatException, "Failed to execute chat {UserId}, {ConversationId}", userId, conversationId);

            throw chatException;
        }
    }
}