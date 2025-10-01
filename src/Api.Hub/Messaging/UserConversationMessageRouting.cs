using Api.Hub.Dto;
using Api.Hub.Interfaces;
using Api.Hub.User;
using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Api.Hub.Messaging
{
    public class UserConversationMessageRouting(IHubContext<UserHub> hub, IUserConnectionManager userConnectionManager) : IMessageRouting
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
            var conversation = JsonSerializer.Deserialize<ConversationStreamingMessage>(message.Body, SerializerOptions);

            if (conversation == null)
            {
                throw new Exception("Failed to deserialize message");
            }

            var connectionIds = userConnectionManager.GetConnections(conversation.UserId);

            foreach (var connectionId in connectionIds)
            {
                await hub.Clients.Client(connectionId).SendAsync("chat", new ChatResponseDto(conversation.ExchangeId, conversation.Message, conversation.ConversationId));
            }
        }
    }
}
