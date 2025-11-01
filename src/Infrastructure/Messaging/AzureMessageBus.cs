using Application.Interfaces;
using Azure.Messaging.ServiceBus;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Infrastructure.Messaging;

public class AzureMessageBus(ServiceBusClient serviceBusClient) : IIntegrationMessaging
{
    private readonly ServiceBusSender _senderQueue = serviceBusClient.CreateSender("agent-queue");
  
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter() }
    };

    public async Task SendAgentMessageAsync<T>(T payload)
    {
        var serializedConversation = JsonSerializer.Serialize(payload, SerializerOptions);

        var serviceBusMessage = new ServiceBusMessage(serializedConversation);
   
        await _senderQueue.SendMessageAsync(serviceBusMessage);
    }
}