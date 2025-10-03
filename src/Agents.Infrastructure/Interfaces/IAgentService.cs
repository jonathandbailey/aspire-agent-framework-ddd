using Azure.Messaging.ServiceBus;

namespace Agents.Infrastructure.Interfaces;

public interface IAgentService
{
    Task ProcessAsync(ServiceBusReceivedMessage message);
}