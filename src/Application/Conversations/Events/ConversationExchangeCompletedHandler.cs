using System.Text;
using Application.Interfaces;
using Domain.Conversations;
using Domain.Events;
using Domain.Interfaces;
using MediatR;

namespace Application.Conversations.Events;

public class ConversationExchangeCompletedHandler(IConversationRepository conversationRepository, IConversationDomainService conversationDomainService,
    IAgentFactory agentFactory) : INotificationHandler<ConversationExchangeCompletedEvent>
{
    public async Task Handle(ConversationExchangeCompletedEvent request, CancellationToken cancellationToken)
    {
        var conversation = await conversationRepository.LoadAsync(request.UserId.Value, request.ConversationId);

        if(!conversation.IsFirstExchange(request.ExchangeId, request.ThreadId))
            return;

        var title = await GenerateTitleAsync(conversation);

        conversation.UpdateTitle(title);

        await conversationRepository.SaveAsync(conversation);
        
    }

    private async Task<string> GenerateTitleAsync(Conversation conversation)
    {
        var agent = await agentFactory.CreateAgent("Title");

        var messages = conversationDomainService.GetMessages(conversation);

        var stringBuilder = new StringBuilder();

        await foreach (var response in agent.InvokeStreamAsync(messages))
        {
            stringBuilder.Append(response.Content);
        }

        return stringBuilder.ToString();
    }
}