using Application.Extensions;
using Application.Interfaces;
using Domain.Conversations;
using MediatR;

namespace Infrastructure.Adapters;

public class ConversationRepositoryDomainAdapter(IMediator mediator, IConversationRepository conversationRepository) : IConversationRepository
{
    public async Task SaveAsync(Conversation conversation)
    {
        await conversationRepository.SaveAsync(conversation);

        await mediator.PublishDomainEvents(conversation);
    }

    public async Task<Conversation> LoadAsync(Guid userId, Guid conversationId)
    {
        return await conversationRepository.LoadAsync(userId, conversationId);
    }
}