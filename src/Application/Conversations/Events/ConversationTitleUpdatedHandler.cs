using Application.Events.Integration;
using Application.Interfaces;
using Domain.Events;
using MediatR;

namespace Application.Conversations.Events;

public class ConversationTitleUpdatedHandler(IConversationClient client, IStreamingEventPublisher publisher) : INotificationHandler<ConversationTitleUpdatedEvent>
{
    public async Task Handle(ConversationTitleUpdatedEvent notification, CancellationToken cancellationToken)
    {
        await publisher.Send(new ConversationTitleUpdateApplicationEvent(notification.UserId, notification.ConversationId, notification.Title));
    }
}