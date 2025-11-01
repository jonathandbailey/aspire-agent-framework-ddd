using Application.Dto;
using Application.Events.Integration;
using Azure.Messaging.ServiceBus;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.Json.Serialization;
using Infrastructure.Settings;

namespace Infrastructure.Messaging;

public class IntegrationMessageWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<IntegrationMessageWorker> _logger;
    private readonly ServiceBusProcessor _processor;

    private readonly ServiceBusProcessor _processorTitle;

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter() }
    };

    public IntegrationMessageWorker(ServiceBusClient serviceBusClient, IServiceProvider serviceProvider, IOptions<TopicSettings> settings,
        ILogger<IntegrationMessageWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;

        _processor = serviceBusClient.CreateProcessor(settings.Value.Domain, settings.Value.DomainSubscriptionExchange);
        _processorTitle = serviceBusClient.CreateProcessor(settings.Value.Domain, settings.Value.DomainSubscriptionTitle);
        _processorTitle.ProcessMessageAsync += ProcessorTitleOnProcessMessageAsync;
        _processorTitle.ProcessErrorAsync += _processorTitle_ProcessErrorAsync;
        _processor.ProcessMessageAsync += OnMessageAsync;
        _processor.ProcessErrorAsync += ErrorHandler;
    }

    private async Task OnMessageAsync(ProcessMessageEventArgs args)
    {
        var message = JsonSerializer.Deserialize<ConversationDomainMessage>(args.Message.Body, SerializerOptions);

        if (message == null)
        {
            throw new Exception("Failed to deserialize the Conversation Domain Message");
        }

        using var scope = _serviceProvider.CreateScope();

        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        await mediator.Send(new ConversationExchangeCompletedIntegrationEvent(message.UserId, message.Content,
            message.ConversationId, message.ExchangeId));


        await args.CompleteMessageAsync(args.Message);
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

        await mediator.Send(new ConversationTitleUpdatedCompletedIntegrationEvent(messageContent.UserId, messageContent.ConversationId,
            messageContent.Content));


        await args.CompleteMessageAsync(args.Message);
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
        await _processor.StartProcessingAsync(stoppingToken);
        await _processorTitle.StartProcessingAsync(stoppingToken);
    }
}