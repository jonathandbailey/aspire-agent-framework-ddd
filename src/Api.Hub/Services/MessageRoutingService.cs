using Api.Hub.Interfaces;
using Api.Hub.Messaging;
using Api.Hub.User;
using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.SignalR;

namespace Api.Hub.Services;

public class MessageRoutingService(IHubContext<UserHub> hub, IUserConnectionManager userConnectionManager) : IMessageRoutingService
{
    private readonly Dictionary<string, IMessageRouting> _messageRoutings = new()
    {
        {"UserConversationStream", new UserConversationMessageRouting(hub, userConnectionManager)},
        {"ConversationTitleStream", new ConversationTitleMessageRouting(hub, userConnectionManager)}
    };
    
    public async Task RouteAsync(string target, ServiceBusReceivedMessage message)
    {
        var messageRouter = _messageRoutings[target];

        await messageRouter.RouteAsync(message);
    }
}