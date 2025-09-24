using Application.Dto;
using Application.Interfaces;
using Domain.Events;
using MediatR;

namespace Application.Conversations.Events;

public class ConversationTitleUpdatedHandler(IConversationClient client) : INotificationHandler<ConversationTitleUpdatedEvent>
{
    public async Task Handle(ConversationTitleUpdatedEvent notification, CancellationToken cancellationToken)
    {
        await client.ExecuteCommand(notification.UserId.Value, new ClientCommandUpdateConversationTitle(notification.ConversationId, notification.Title));
    }
}