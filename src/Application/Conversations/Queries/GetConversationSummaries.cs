using Application.Dto;
using Application.Interfaces;
using MediatR;

namespace Application.Conversations.Queries;

public class GetConversationSummaries(IConversationQueries conversationQueries) : IRequestHandler<GetConversationSummariesQuery, List<ConversationSummaryItem>>
{
    public async Task<List<ConversationSummaryItem>> Handle(GetConversationSummariesQuery request, CancellationToken cancellationToken)
    {
        return await conversationQueries.GetConversationSummaries(request.UserId);
    }
}

public sealed record GetConversationSummariesQuery(Guid UserId) : IRequest<List<ConversationSummaryItem>>;