using Application.Dto;
using Application.Interfaces;
using Azure.Messaging.ServiceBus;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Infrastructure.Messaging;

public class AzureMessageBus(ServiceBusClient serviceBusClient) : IMessageBus
{
    private readonly ServiceBusSender _sender = serviceBusClient.CreateSender("topic");

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter() }
    };

    public async Task PublishToUser(ConversationStreamingMessage payload)
    {
        var serializedConversation = JsonSerializer.Serialize(payload, SerializerOptions);

        await _sender.SendMessageAsync(new ServiceBusMessage(serializedConversation));
    }
}