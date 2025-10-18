using Application.Dto;
using Application.Interfaces;
using Domain.Conversations;
using MediatR;

namespace Application.Events.Integration;

public class ConversationTitleUpdateStartedEventHandler(IIntegrationMessaging messageBus, IConversationRepository conversationRepository) : 
    IRequestHandler<ConversationTitleUpdateStartedIntegrationEvent>, 
    IRequestHandler<ConversationTitleUpdatedCompletedIntegrationEvent>
{
    public async Task Handle(ConversationTitleUpdateStartedIntegrationEvent request, CancellationToken cancellationToken)
    {
        await messageBus.SendAsyncToSummarize(new ConversationTitleUpdateStartedMessage(request.UserId, request.ConversationId,
            request.Messages));
    }

    public async Task Handle(ConversationTitleUpdatedCompletedIntegrationEvent request, CancellationToken cancellationToken)
    {
        var conversation = await conversationRepository.LoadAsync(request.UserId, request.ConversationId);

        conversation.UpdateTitle(request.Title);

        await conversationRepository.SaveAsync(conversation);
    }
}

public sealed record ConversationTitleUpdateStartedIntegrationEvent(Guid UserId, Guid ConversationId, List<Message> Messages) : IRequest;

public sealed record ConversationTitleUpdatedCompletedIntegrationEvent(Guid UserId, Guid ConversationId, string Title) : IRequest;