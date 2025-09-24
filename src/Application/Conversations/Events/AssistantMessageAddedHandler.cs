using Application.Interfaces;
using Domain.Events;
using MediatR;

namespace Application.Conversations.Events;

public class AssistantMessageAddedHandler(IConversationRepository conversationRepository,
    IAssistantFactory assistantFactory) : INotificationHandler<ConversationTurnEndedEvent>
{
    public async Task Handle(ConversationTurnEndedEvent request, CancellationToken cancellationToken)
    {
        var conversation = await conversationRepository.LoadAsync(request.UserId.Value, request.ConversationId);

        if (string.IsNullOrWhiteSpace(conversation.Name))
        {
            var titleAssistant = await assistantFactory.CreateTitleAssistant();

            var threadSummary = conversation.GetConversationSummaryForTitleGeneration();
        
            var response = await titleAssistant.InvokeAsync(threadSummary);
          
            conversation.UpdateTitle(response.Content);

            await conversationRepository.SaveAsync(conversation);
        }
    }
}