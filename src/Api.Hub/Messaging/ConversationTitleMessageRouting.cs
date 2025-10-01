using Api.Hub.Dto;
using Api.Hub.Interfaces;
using Api.Hub.User;
using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Api.Hub.Messaging;

public class ConversationTitleMessageRouting(IHubContext<UserHub> hub, IUserConnectionManager userConnectionManager) : IMessageRouting
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter() }
    };

    public async Task RouteAsync(ServiceBusReceivedMessage message)
    {
        var titleUpdatedMessage = JsonSerializer.Deserialize<ConversationTitleUpdatedMessage>(message.Body, SerializerOptions);

        if (titleUpdatedMessage == null)
        {
            throw new Exception("Failed to deserialize message");
        }

        var connectionIds = userConnectionManager.GetConnections(titleUpdatedMessage.UserId);

        foreach (var connectionId in connectionIds)
        {
            await hub.Clients.Client(connectionId).SendAsync("command", new ClientCommandUpdateConversationTitle(titleUpdatedMessage.ConversationId, titleUpdatedMessage.Content));
        }
    }
}