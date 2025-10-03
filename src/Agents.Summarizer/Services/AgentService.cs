using Agents.Infrastructure.Interfaces;
using Azure.Messaging.ServiceBus;

namespace Agents.Summarizer.Services;

public class AgentService : IAgentService
{
    public async Task ProcessAsync(ServiceBusReceivedMessage message)
    {
        throw new NotImplementedException();
    }
}