using Application.Dto;
using Application.Interfaces;
using Azure.Messaging.ServiceBus;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Infrastructure.Messaging;

public class AzureMessageBus(ServiceBusClient serviceBusClient) : IMessageBus
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter() }
    };

    public async Task PublishToUser(ConversationStreamingMessage payload)
    {
        var sender = serviceBusClient.CreateSender("topic");

        var serializedConversation = JsonSerializer.Serialize(payload, SerializerOptions);

        using var messageBatch =
            await sender.CreateMessageBatchAsync();

        if (!messageBatch.TryAddMessage(
                new ServiceBusMessage(serializedConversation)))
        {
            throw new Exception($"The payload is too large to fit the batch.");
        }

        await sender.SendMessagesAsync(messageBatch);
    }
}