using Application.Interfaces;
using FluentValidation;
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

public sealed class StartConversationExchangeCommandValidator : AbstractValidator<StartConversationExchangeCommand>
{
    public StartConversationExchangeCommandValidator()
    {
        RuleFor(v => v.Message)
            .NotEmpty()
            .NotNull();

        RuleFor(v => v.UserId).NotEmpty();
        RuleFor(v => v.ConversationId).NotEmpty();
    }
}