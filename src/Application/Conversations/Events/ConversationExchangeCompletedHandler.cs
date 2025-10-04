using Application.Events.Integration;
using Application.Interfaces;
using Domain.Conversations;
using Domain.Events;
using Domain.Interfaces;
using MediatR;
using System.Text;

namespace Application.Conversations.Events;

public class ConversationExchangeCompletedHandler(IConversationRepository conversationRepository, IConversationDomainService conversationDomainService,
    IAgentFactory agentFactory, IMediator mediator) : INotificationHandler<ConversationExchangeCompletedEvent>
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

        await mediator.Send(new ConversationTitleEvent(conversation.UserId.Value, conversation.Id, messages));

        var stringBuilder = new StringBuilder();

        await foreach (var response in agent.InvokeStreamAsync(messages))
        {
            stringBuilder.Append(response.Content);
        }

        return stringBuilder.ToString();
    }
}