using Agents.Infrastructure.Interfaces;
using Agents.Infrastructure.Settings;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Agents.Infrastructure.Services;

public class MessagingWorker : BackgroundService
{
    private readonly ILogger<MessagingWorker> _logger;
    private readonly IAgentService _agentService;
    private readonly ServiceBusProcessor _processor;

    public MessagingWorker(ServiceBusClient serviceBusClient, ILogger<MessagingWorker> logger, IAgentService agentService, IOptions<QueueSettings> settings)
    {
        _logger = logger;
        _agentService = agentService;
        _processor = serviceBusClient.CreateProcessor(settings.Value.Agent, new ServiceBusProcessorOptions());
        _processor.ProcessMessageAsync += OnMessageAsync;
        _processor.ProcessErrorAsync += ErrorHandler;
    }

    private async Task OnMessageAsync(ProcessMessageEventArgs args)
    {
        await _agentService.ProcessAsync(args.Message);

        await args.CompleteMessageAsync(args.Message);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _processor.StartProcessingAsync(stoppingToken);
     }

    private Task ErrorHandler(ProcessErrorEventArgs args)
    {
        _logger.LogError(args.Exception, "Conversation Message Worker Failed to Process Messages.");
        return Task.CompletedTask;
    }
}
