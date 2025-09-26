using Application.Dto;
using Application.Interfaces;
using MediatR;

namespace Application.Conversations.Queries;

public class GetConversations(IConversationQueries conversationQueries) : IRequestHandler<GetConversationsQuery, List<Conversation>>
{
    public async Task<List<Conversation>> Handle(GetConversationsQuery request, CancellationToken cancellationToken)
    {
        return await conversationQueries.GetAllConversationsAsync(request.UserId);
    }
}

public sealed record GetConversationsQuery(Guid UserId) : IRequest<List<Conversation>>;