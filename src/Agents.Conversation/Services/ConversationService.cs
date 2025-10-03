using Agents.Conversation.Interfaces;
using Azure.Messaging.ServiceBus;
using System.Text.Json;
using System.Text.Json.Serialization;
using Agents.Infrastructure.Dto;

namespace Agents.Conversation.Services;

public class ConversationService(ServiceBusClient serviceBusClient) : IConversationService
{
    private readonly ServiceBusSender _conversationDomainSender = serviceBusClient.CreateSender("conversation-domain-queue");

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter() }
    };

    public async Task PublishDomainUpdate(Guid userId, string content, Guid conversationId, Guid exchangeId)
    {
        var message = new ConversationDomainMessage(userId, content,
            conversationId, exchangeId);

        var serializedDomainMessage = JsonSerializer.Serialize(message, SerializerOptions);

        await _conversationDomainSender.SendMessageAsync(new ServiceBusMessage(serializedDomainMessage));
    }
}