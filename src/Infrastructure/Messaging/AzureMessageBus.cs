using Application.Interfaces;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.Json.Serialization;
using Infrastructure.Settings;

namespace Infrastructure.Messaging;

public class AzureMessageBus(ServiceBusClient serviceBusClient, IOptions<QueueSettings> settings) : IIntegrationMessaging
{
    private readonly ServiceBusSender _senderQueue = serviceBusClient.CreateSender(settings.Value.Agent);
  
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