using Application.Dto;
using Application.Interfaces;
using MediatR;

namespace Application.Conversations.Queries;

public class GetConversationById(IConversationQuery conversationQuery) : IRequestHandler<GetConversationByIdQuery, Conversation>
{
    public async Task<Conversation> Handle(GetConversationByIdQuery request, CancellationToken cancellationToken)
    {
        return await conversationQuery.LoadAsync(request.UserId, request.ConversationId);
    }
}

public sealed record GetConversationByIdQuery(Guid UserId, Guid ConversationId) : IRequest<Conversation>;