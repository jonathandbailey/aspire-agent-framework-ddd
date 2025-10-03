using Agents.Conversation.Dto;
using Agents.Conversation.Interfaces;
using Azure.Messaging.ServiceBus;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Agents.Conversation.Services;

public class AgentService(ServiceBusClient serviceBusClient, IAgentFactory agentFactory)
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

    public async Task<AssistantResponseDto> ProcessAsync(ConversationAgentMessage conversation)
    {
        var frameworkAgent = await agentFactory.CreateAgent();

        var stringBuilder = new StringBuilder();

        await foreach (var response in frameworkAgent.InvokeStreamAsync(conversation.Messages))
        {
            stringBuilder.Append(response.Content);

            var payload = new ConversationStreamingMessage(conversation.UserId, response.Content,
                conversation.ConversationId, conversation.ExchangeId);

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

        return new AssistantResponseDto() { Content = stringBuilder.ToString() };
    }
}