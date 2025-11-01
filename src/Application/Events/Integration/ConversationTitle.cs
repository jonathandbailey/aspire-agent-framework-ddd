using Application.Interfaces;
using MediatR;

namespace Application.Events.Integration;

public class ConversationTitleUpdateStartedEventHandler(IConversationRepository conversationRepository) : 
    IRequestHandler<ConversationTitleUpdatedCompletedIntegrationEvent>
{
   

    public async Task Handle(ConversationTitleUpdatedCompletedIntegrationEvent request, CancellationToken cancellationToken)
    {
        var conversation = await conversationRepository.LoadAsync(request.UserId, request.ConversationId);

        conversation.UpdateTitle(request.Title);

        await conversationRepository.SaveAsync(conversation);
    }
}

public sealed record ConversationTitleUpdatedCompletedIntegrationEvent(Guid UserId, Guid ConversationId, string Title) : IRequest;