using Api.Hub.Interfaces;
using Azure.Messaging.ServiceBus;

namespace Api.Hub.Services;

public class ConversationMessageWorker : BackgroundService, IAsyncDisposable
{
    private readonly IMessageRoutingService _messageRoutingService;
    private readonly ILogger<ConversationMessageWorker> _logger;
    private readonly ServiceBusProcessor _processor;

 
    public ConversationMessageWorker(ServiceBusClient serviceBusClient, IMessageRoutingService messageRoutingService, ILogger<ConversationMessageWorker> logger)
    {
        _messageRoutingService = messageRoutingService;
        _logger = logger;
        _processor = serviceBusClient.CreateProcessor("user-topic", "subscription");
        _processor.ProcessMessageAsync += OnMessageAsync;
        _processor.ProcessErrorAsync += ErrorHandler;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _processor.StartProcessingAsync(stoppingToken);
    }

    private async Task OnMessageAsync(ProcessMessageEventArgs args)
    {
        var target = args.Message.ApplicationProperties["Target"].ToString();

        if (string.IsNullOrWhiteSpace(target))
            throw new InvalidOperationException("Message missing or empty 'Target' application property.");

        await _messageRoutingService.RouteAsync(target, args.Message);
   
        await args.CompleteMessageAsync(args.Message);
    }

    private Task ErrorHandler(ProcessErrorEventArgs args)
    {
        _logger.LogError(args.Exception, "Conversation Message Worker Failed to Process Messages." );
        return Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _processor.ProcessMessageAsync -= OnMessageAsync;
        _processor.ProcessErrorAsync -= ErrorHandler;
        await _processor.StopProcessingAsync(cancellationToken);
        await base.StopAsync(cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        await _processor.DisposeAsync();
    }
}