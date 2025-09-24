using FluentValidation;

namespace Application.Conversations.Queries;

public sealed class GetConversationByIdQueryValidator : AbstractValidator<GetConversationByIdQuery>
{
    public GetConversationByIdQueryValidator()
    {
        RuleFor(v => v.UserId).NotEmpty();
        RuleFor(v => v.ConversationId).NotEmpty();
    }
}

public sealed class GetConversationsQueryValidator : AbstractValidator<GetConversationsQuery>
{
    public GetConversationsQueryValidator()
    {
        RuleFor(v => v.UserId).NotEmpty();
    }
}

public sealed class GetConversationSummariesValidator : AbstractValidator<GetConversationSummariesQuery>
{
    public GetConversationSummariesValidator()
    {
        RuleFor(v => v.UserId).NotEmpty();
    }
}