using Application.Interfaces;
using Domain.Events;
using Domain.Interfaces;
using MediatR;

namespace Application.Conversations.Events;

public class AssistantMessageAddedHandler(IConversationRepository conversationRepository, IConversationDomainService conversationDomainService,
    IAgentFactory agentFactory) : INotificationHandler<ConversationTurnEndedEvent>
{
    public async Task Handle(ConversationTurnEndedEvent request, CancellationToken cancellationToken)
    {
        var conversation = await conversationRepository.LoadAsync(request.UserId.Value, request.ConversationId);

        if (string.IsNullOrWhiteSpace(conversation.Name))
        {
            var titleAssistant = await agentFactory.CreateTitleAssistant();

            var threadSummary = conversationDomainService.GetConversationSummary(conversation);
        
            var response = await titleAssistant.InvokeAsync(threadSummary);
          
            conversation.UpdateTitle(response.Content);

            await conversationRepository.SaveAsync(conversation);
        }
    }
}