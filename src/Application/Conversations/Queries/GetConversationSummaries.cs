using Application.Dto;
using Application.Interfaces;
using MediatR;

namespace Application.Conversations.Queries;

public class GetConversationSummaries(IConversationQuery conversationQuery) : IRequestHandler<GetConversationSummariesQuery, List<ConversationSummaryItem>>
{
    public async Task<List<ConversationSummaryItem>> Handle(GetConversationSummariesQuery request, CancellationToken cancellationToken)
    {
        return await conversationQuery.GetConversationSummaries(request.UserId);
    }
}

public sealed record GetConversationSummariesQuery(Guid UserId) : IRequest<List<ConversationSummaryItem>>;