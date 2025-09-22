using System.Text;
using Application.Extensions;
using Application.Interfaces;
using Domain.Conversations;
using Domain.Events;
using MediatR;

namespace Application.Events;

public class AssistantMessageAddedHandler(IConversationRepository conversationRepository,
    IMediator mediator,
    IAssistantFactory assistantFactory) : INotificationHandler<AssistantMessageAddedEvent>
{
    public async Task Handle(AssistantMessageAddedEvent request, CancellationToken cancellationToken)
    {
        var conversation = await conversationRepository.LoadAsync(request.UserId, request.ConversationId);

        if (string.IsNullOrWhiteSpace(conversation.Name))
        {
            var titleAssistant = await assistantFactory.CreateTitleAssistant();

            var firstThread = conversation.Threads.FirstOrDefault();

            if (firstThread == null)
            {
                throw new InvalidOperationException($"Conversation {conversation.Id} doesn't have any threads");
            }

            

            var userMessage = new UserMessage(string.Empty, 0);

            await foreach (var response in titleAssistant.StreamAsync(userMessage))
            {
                if (JsonOutputParser.HasJson(response))
                {
                    var conversationTitle = JsonOutputParser.Parse<ConversationTitle>(response);

                    conversation.UpdateTitle(conversationTitle.Title, request.UserId);
                }
            }

            await conversationRepository.SaveAsync(conversation);

            await mediator.PublishDomainEvents(conversation);
        }
    }
}