using Application.Interfaces;
using Azure.Messaging.ServiceBus;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Infrastructure.Messaging;

public class AzureMessageBus(ServiceBusClient serviceBusClient) : IMessageBus
{
    private readonly ServiceBusSender _sender = serviceBusClient.CreateSender("user-topic");
    private readonly ServiceBusSender _senderQueue = serviceBusClient.CreateSender("agent-conversation-queue");
    private readonly ServiceBusSender _senderQueueTitle = serviceBusClient.CreateSender("agent-summarizer-queue");

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter() }
    };

    public async Task SendAsync<T>(T payload, string target)
    {
        var serializedConversation = JsonSerializer.Serialize(payload, SerializerOptions);

        var serviceBusMessage = new ServiceBusMessage(serializedConversation) { ApplicationProperties =
        {
            { "Target" , target}
        }};

        await _sender.SendMessageAsync(serviceBusMessage);
    }

    public async Task SendAsync<T>(T payload)
    {
        var serializedConversation = JsonSerializer.Serialize(payload, SerializerOptions);

        var serviceBusMessage = new ServiceBusMessage(serializedConversation);
   
        await _senderQueue.SendMessageAsync(serviceBusMessage);
    }

    public async Task SendAsyncToSummarize<T>(T payload)
    {
        var serializedConversation = JsonSerializer.Serialize(payload, SerializerOptions);

        var serviceBusMessage = new ServiceBusMessage(serializedConversation);

        await _senderQueueTitle.SendMessageAsync(serviceBusMessage);
    }
}