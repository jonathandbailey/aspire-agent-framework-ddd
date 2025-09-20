using Api.Extensions;
using Application.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace Api.Hubs;

public class ChatHub(IUserConnectionManager userConnectionManager, ILogger<ChatHub> logger) : Hub
{
    public override async Task OnConnectedAsync()
    {
        try
        {
            var userId = GetUserId();
            userConnectionManager.AddConnection(userId, Context.ConnectionId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to add connection for ConnectionId: {ConnectionId}", Context.ConnectionId);
            throw;
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        try
        {
            userConnectionManager.RemoveConnection(Context.ConnectionId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to remove connection for ConnectionId: {ConnectionId}", Context.ConnectionId);
        }

        await base.OnDisconnectedAsync(exception);
    }

    private Guid GetUserId()
    {
        if (Context.User == null)
        {
            logger.LogError("Chat Hub cannot access User in Context. ConnectionId: {ConnectionId}", Context.ConnectionId);
            throw new InvalidOperationException($"Chat Hub cannot access User in Context for ConnectionId: {Context.ConnectionId}");
        }

        return Context.User.Id();
    }
}

