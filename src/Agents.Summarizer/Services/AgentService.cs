using Agents.Infrastructure.Dto;
using Agents.Infrastructure.Interfaces;
using Agents.Infrastructure.Settings;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Agents.Conversation.Common;

namespace Agents.Summarizer.Services;

public class AgentService(ServiceBusClient serviceBusClient, IAgentFactory agentFactory, IOptions<TopicSettings> settings) : IAgentService
{
    private readonly ServiceBusSender _userStreamSender = serviceBusClient.CreateSender(settings.Value.User);
    private readonly ServiceBusSender _conversationDomainSender = serviceBusClient.CreateSender(settings.Value.Domain);

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter() }
    };

    public async Task ProcessAsync(ServiceBusReceivedMessage message)
    {
        var agentConversationRequest = JsonSerializer.Deserialize<ConversationTitleMessage>(message.Body, SerializerOptions);

        if (agentConversationRequest == null)
        {
            throw new Exception("Failed to deserialize conversation.");
        }

        var frameworkAgent = await agentFactory.CreateWrappedAgent(InfrastructureConstants.TitleAssistantName);

        var stringBuilder = new StringBuilder();

        await foreach (var response in frameworkAgent.InvokeStreamAsync(agentConversationRequest.Messages))
        {
            stringBuilder.Append(response.Content);

            var payload = new ConversationTitleUpdatedMessage(agentConversationRequest.UserId, agentConversationRequest.ConversationId, response.Content);

            var serializedConversation = JsonSerializer.Serialize(payload, SerializerOptions);

            var serviceBusMessage = new ServiceBusMessage(serializedConversation)
            {
                ApplicationProperties =
                {
                    { "Target" , "ConversationTitleStream"}
                }
            };

            await _userStreamSender.SendMessageAsync(serviceBusMessage);
        }

        var titleUpdatedMessage = new ConversationTitleUpdatedMessage(
            agentConversationRequest.UserId, agentConversationRequest.ConversationId, stringBuilder.ToString());

        var serializedDomainMessage = JsonSerializer.Serialize(titleUpdatedMessage, SerializerOptions);

        await _conversationDomainSender.SendMessageAsync(new ServiceBusMessage(serializedDomainMessage) { Subject = "TitleUpdate" });
    }
}