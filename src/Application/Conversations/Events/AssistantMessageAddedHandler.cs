using System.Text;
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
            var titleAssistant = await agentFactory.CreateAgent("Title");

            var threadSummary = conversationDomainService.GetMessages(conversation);

            var stringBuilder = new StringBuilder();

            await foreach (var response in titleAssistant.InvokeStreamAsync(threadSummary))
            {
                stringBuilder.Append(response.Content);
            }
          
            conversation.UpdateTitle(stringBuilder.ToString());

            await conversationRepository.SaveAsync(conversation);
        }
    }
}