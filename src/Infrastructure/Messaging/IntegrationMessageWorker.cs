using Application.Dto;
using Application.Interfaces;
using Application.Messaging;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;
using Application.Events.Integration;
using MediatR;

namespace Infrastructure.Messaging;

public class IntegrationMessageWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ConversationWorker> _logger;
    private readonly ServiceBusProcessor _processor;

    private readonly ServiceBusProcessor _processorTitle;

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter() }
    };

    public IntegrationMessageWorker(ServiceBusClient serviceBusClient, IServiceProvider serviceProvider,
        ILogger<ConversationWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;

        _processor = serviceBusClient.CreateProcessor("conversation-domain-topic", "exchange-complete-subscription");
        _processorTitle = serviceBusClient.CreateProcessor("conversation-domain-topic", "title-update-subscription");
        _processorTitle.ProcessMessageAsync += ProcessorTitleOnProcessMessageAsync;
        _processorTitle.ProcessErrorAsync += _processorTitle_ProcessErrorAsync;
        _processor.ProcessMessageAsync += OnMessageAsync;
        _processor.ProcessErrorAsync += ErrorHandler;
    }

    private async Task OnMessageAsync(ProcessMessageEventArgs args)
    {

    }

    private async Task ProcessorTitleOnProcessMessageAsync(ProcessMessageEventArgs args)
    {
        var messageContent = JsonSerializer.Deserialize<ConversationTitleUpdatedMessage>(args.Message.Body, SerializerOptions);

        if (messageContent == null)
        {
            throw new Exception("Failed to deserialize the Conversation Domain Message");
        }

        using var scope = _serviceProvider.CreateScope();

        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        await mediator.Send(new ConversationTitleUpdatedEvent(messageContent.UserId, messageContent.ConversationId,
            messageContent.Content));
    }

    private Task _processorTitle_ProcessErrorAsync(ProcessErrorEventArgs arg)
    {
        _logger.LogError(arg.Exception, "Conversation Message Worker Failed to Process Messages.");
        return Task.CompletedTask;
    }

    private Task ErrorHandler(ProcessErrorEventArgs args)
    {
        _logger.LogError(args.Exception, "Conversation Message Worker Failed to Process Messages.");
        return Task.CompletedTask;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        //await _processor.StartProcessingAsync(stoppingToken);
        await _processorTitle.StartProcessingAsync(stoppingToken);
    }
}