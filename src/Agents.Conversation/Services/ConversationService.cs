using Agents.Conversation.Interfaces;
using Agents.Infrastructure.Dto;
using Agents.Infrastructure.Settings;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Agents.Conversation.Services;

public class ConversationService(ServiceBusClient serviceBusClient, IOptions<QueueSettings> settings) : IConversationService
{
    private readonly ServiceBusSender _conversationDomainSender = serviceBusClient.CreateSender(settings.Value.Domain);

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