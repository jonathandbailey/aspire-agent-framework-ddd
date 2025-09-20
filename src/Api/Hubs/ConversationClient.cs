using Application.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace Api.Hubs;

public class ConversationClient(IHubContext<ChatHub> hub, IUserConnectionManager userConnectionManager) : IConversationClient
{
    private const string SendMethodChat = "chat";
    private const string SendMethodCommand = "command";

    public async Task ChatWithUser<T>(Guid userId, T payload)
    {
        await Send(SendMethodChat, userId, payload);
    }

    public async Task ExecuteCommand<T>(Guid userId, T payload)
    {
        await Send(SendMethodCommand, userId, payload);
    }

    private async Task Send<T>(string method, Guid userId, T payload)
    {
        Verify.NotNullOrWhiteSpace(method);
        Verify.NotEmpty(userId);
        Verify.NotNull(payload);

        var connectionIds = userConnectionManager.GetConnections(userId);

        foreach (var connectionId in connectionIds)
        {
            await hub.Clients.Client(connectionId).SendAsync(method, payload);
        }
    }
}