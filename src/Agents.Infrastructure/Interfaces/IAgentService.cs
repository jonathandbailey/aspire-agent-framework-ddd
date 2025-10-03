using Agents.Infrastructure.Dto;
using Azure.Messaging.ServiceBus;

namespace Agents.Infrastructure.Interfaces;

public interface IAgentService
{
    Task<AssistantResponseDto> ProcessAsync(ConversationAgentMessage agentConversationRequest);
    Task ProcessAsync(ServiceBusReceivedMessage message);
}