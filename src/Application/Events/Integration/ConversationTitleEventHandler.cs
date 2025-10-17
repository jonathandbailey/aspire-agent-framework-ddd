using Application.Dto;
using Application.Interfaces;
using MediatR;

namespace Application.Events.Integration;

public class ConversationTitleEventHandler(IIntegrationMessaging messageBus, IConversationRepository conversationRepository) : 
    IRequestHandler<ConversationTitleEvent>, 
    IRequestHandler<ConversationTitleUpdatedEvent>
{
    public async Task Handle(ConversationTitleEvent request, CancellationToken cancellationToken)
    {
        await messageBus.SendAsyncToSummarize(new ConversationTitleMessage(request.UserId, request.ConversationId,
            request.Messages));
    }

    public async Task Handle(ConversationTitleUpdatedEvent request, CancellationToken cancellationToken)
    {
        var conversation = await conversationRepository.LoadAsync(request.UserId, request.ConversationId);

        conversation.UpdateTitle(request.Title);

        await conversationRepository.SaveAsync(conversation);
    }
}