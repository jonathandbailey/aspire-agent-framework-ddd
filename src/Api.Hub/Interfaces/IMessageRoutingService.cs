using Azure.Messaging.ServiceBus;

namespace Api.Hub.Interfaces;

public interface IMessageRoutingService
{
    Task RouteAsync(string target, ServiceBusReceivedMessage message);
}