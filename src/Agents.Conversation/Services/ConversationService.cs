using Agents.Conversation.Interfaces;
using Agents.Infrastructure.Dto;
using Agents.Infrastructure.Settings;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Agents.Conversation.Services;

public class ConversationService(ServiceBusClient serviceBusClient, IOptions<TopicSettings> settings) : IConversationService
{
    private readonly ServiceBusSender _conversationDomainSender = serviceBusClient.CreateSender(settings.Value.Domain);
    private readonly ServiceBusSender _userStreamSender = serviceBusClient.CreateSender(settings.Value.User);

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

        await _conversationDomainSender.SendMessageAsync(new ServiceBusMessage(serializedDomainMessage){ Subject = "ExchangeComplete" });
    }

    public async Task PublishUserStream(Guid userId, string content, Guid conversationId, Guid exchangeId)
    {
        var payload = new ConversationStreamingMessage(userId, content,
            conversationId, exchangeId);

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

    public async Task PublishConversationTitleStream(Guid userId, string content, Guid conversationId)
    {
        var payload = new ConversationTitleUpdatedMessage(userId, conversationId, content);

        var serializedConversation = JsonSerializer.Serialize(payload, SerializerOptions);

        var serviceBusMessage = new ServiceBusMessage(serializedConversation)
        {
            ApplicationProperties =
            {
                { "Target" , "ConversationTitleStream"}
            }
        };

        await _userStreamSender.SendMessageAsync(serviceBusMessage);

        await _conversationDomainSender.SendMessageAsync(new ServiceBusMessage(serializedConversation) { Subject = "TitleUpdate" });
    }
}