using FluentValidation;

namespace Application.Conversations.Commands;

public sealed class StartConversationExchangeCommandValidator : AbstractValidator<StartConversationExchangeCommand>
{
    public StartConversationExchangeCommandValidator()
    {
        RuleFor(v => v.Message)
            .NotEmpty()
            .NotNull();

        RuleFor(v => v.UserId).NotEmpty();
        RuleFor(v => v.ConversationId).NotEmpty();
        RuleFor(v => v.ExchangeId).NotEmpty();
    }
}

public sealed class CreateConversationCommandValidator : AbstractValidator<CreateConversationCommand>
{
    public CreateConversationCommandValidator()
    {
        RuleFor(v => v.UserId).NotEmpty();
    }
}

public sealed class CreateConversationExchangeCommandValidator : AbstractValidator<CreateConversationExchangeCommand>
{
    public CreateConversationExchangeCommandValidator()
    {
        RuleFor(v => v.UserId).NotEmpty();
        RuleFor(v => v.ConversationId).NotEmpty();
    }
}