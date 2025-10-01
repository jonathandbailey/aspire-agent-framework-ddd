using Azure.Messaging.ServiceBus;

namespace Api.Hub.Interfaces;

public interface IMessageRouting
{
    Task RouteAsync(ServiceBusReceivedMessage message);
}