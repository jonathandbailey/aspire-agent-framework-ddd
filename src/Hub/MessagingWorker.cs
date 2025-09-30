using Application.Interfaces;
using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;
using System.Text.Json.Serialization;
using Hub.Dto;

namespace Hub;

public class MessagingWorker : BackgroundService
{
    private readonly IHubContext<ChatHub> _hub;
    private readonly IUserConnectionManager _userConnectionManager;
    private readonly ServiceBusProcessor _processor;

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter() }
    };

    public MessagingWorker(ServiceBusClient serviceBusClient, IHubContext<ChatHub> hub, IUserConnectionManager userConnectionManager)
    {
        _hub = hub;
        _userConnectionManager = userConnectionManager;
        _processor = serviceBusClient.CreateProcessor("topic", "subscription");
        _processor.ProcessMessageAsync += OnMessageAsync;
        _processor.ProcessErrorAsync += ErrorHandler;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _processor.StartProcessingAsync(stoppingToken);
    }

    private async Task OnMessageAsync(ProcessMessageEventArgs args)
    {
        var conversation = JsonSerializer.Deserialize<ConversationStreamingMessage>(args.Message.Body, SerializerOptions);

        if (conversation == null)
        {
            throw new Exception("Failed to deserialize message");
        }

        var connectionIds = _userConnectionManager.GetConnections(conversation.UserId);

        foreach (var connectionId in connectionIds)
        {
            await _hub.Clients.Client(connectionId).SendAsync("chat", new ChatResponseDto(conversation.ExchangeId, conversation.Message, conversation.ConversationId));
        }

        await args.CompleteMessageAsync(args.Message);
    }

    private Task ErrorHandler(ProcessErrorEventArgs args)
    {
        Console.WriteLine(args.Exception);
        return Task.CompletedTask;
    }
}