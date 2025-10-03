using Agents.Conversation.Interfaces;
using Azure.Messaging.ServiceBus;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Agents.Conversation.Common;
using Agents.Infrastructure.Dto;
using Agents.Infrastructure.Interfaces;

namespace Agents.Conversation.Services;

public class AgentService(ServiceBusClient serviceBusClient, IAgentFactory agentFactory, IConversationService conversationService)
    : IAgentService
{
    private readonly ServiceBusSender _userStreamSender = serviceBusClient.CreateSender("topic");

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter() }
    };

    public async Task ProcessAsync(ServiceBusReceivedMessage message)
    {
        var agentConversationRequest = JsonSerializer.Deserialize<ConversationAgentMessage>(message.Body, SerializerOptions);

        if (agentConversationRequest == null)
        {
            throw new Exception("Failed to deserialize conversation.");
        }

        await ProcessAsync(agentConversationRequest);
    }

    public async Task<AssistantResponseDto> ProcessAsync(ConversationAgentMessage agentConversationRequest)
    {
        var frameworkAgent = await agentFactory.CreateAgent(InfrastructureConstants.ChatAgentTemplateName);

        var stringBuilder = new StringBuilder();

        await foreach (var response in frameworkAgent.InvokeStreamAsync(agentConversationRequest.Messages))
        {
            stringBuilder.Append(response.Content);

            var payload = new ConversationStreamingMessage(agentConversationRequest.UserId, response.Content,
                agentConversationRequest.ConversationId, agentConversationRequest.ExchangeId);

            var serializedConversation = JsonSerializer.Serialize(payload, SerializerOptions);

            var serviceBusMessage = new ServiceBusMessage(serializedConversation)
            {
                ApplicationProperties =
                {
                    { "Target" , "UserConversationStream"}
                }
            };

            await _userStreamSender.SendMessageAsync(serviceBusMessage);
        }

        await conversationService.PublishDomainUpdate(agentConversationRequest.UserId, stringBuilder.ToString(),
            agentConversationRequest.ConversationId, agentConversationRequest.ExchangeId);

        return new AssistantResponseDto() { Content = stringBuilder.ToString() };
    }
}