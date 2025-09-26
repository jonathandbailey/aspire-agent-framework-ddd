using Application.Dto;
using Application.Interfaces;
using MediatR;

namespace Application.Conversations.Queries;

public class GetConversationById(IConversationQueries conversationQueries) : IRequestHandler<GetConversationByIdQuery, Conversation>
{
    public async Task<Conversation> Handle(GetConversationByIdQuery request, CancellationToken cancellationToken)
    {
        return await conversationQueries.LoadAsync(request.UserId, request.ConversationId);
    }
}

public sealed record GetConversationByIdQuery(Guid UserId, Guid ConversationId) : IRequest<Conversation>;