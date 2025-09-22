using Application.Dto;
using Application.Interfaces;
using MediatR;

namespace Application.Conversations.Queries;

public class GetConversations(IConversationQuery conversationQuery) : IRequestHandler<GetConversationsQuery, List<Conversation>>
{
    public async Task<List<Conversation>> Handle(GetConversationsQuery request, CancellationToken cancellationToken)
    {
        return await conversationQuery.GetAllConversationsAsync(request.UserId);
    }
}

public sealed record GetConversationsQuery(Guid UserId) : IRequest<List<Conversation>>;