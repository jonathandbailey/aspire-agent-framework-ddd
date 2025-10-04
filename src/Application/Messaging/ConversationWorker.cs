using Application.Dto;
using Application.Interfaces;
using Azure.Core;
using Azure.Messaging.ServiceBus;
using Domain.Conversations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Application.Messaging;

public class ConversationWorker : BackgroundService
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

    public ConversationWorker(ServiceBusClient serviceBusClient, IServiceProvider serviceProvider, ILogger<ConversationWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _processor = serviceBusClient.CreateProcessor("conversation-domain-topic", "exchange-complete-subscription");
        _processorTitle = serviceBusClient.CreateProcessor("conversation-domain-topic", "title-update-subscription");
        _processorTitle.ProcessMessageAsync+= ProcessorTitleOnProcessMessageAsync;
        _processorTitle.ProcessErrorAsync += _processorTitle_ProcessErrorAsync;
        _processor.ProcessMessageAsync += OnMessageAsync;
        _processor.ProcessErrorAsync += ErrorHandler;
    }

    private Task _processorTitle_ProcessErrorAsync(ProcessErrorEventArgs arg)
    {
        _logger.LogError(arg.Exception, "Conversation Message Worker Failed to Process Messages.");
        return Task.CompletedTask;
    }

    private async Task ProcessorTitleOnProcessMessageAsync(ProcessMessageEventArgs args)
    {
        var conversationDomainMessage = JsonSerializer.Deserialize<ConversationTitleUpdatedMessage>(args.Message.Body, SerializerOptions);

        if (conversationDomainMessage == null)
        {
            throw new Exception("Failed to deserialize the Conversation Domain Message");
        }

        using var scope = _serviceProvider.CreateScope();

        var conversationRepository = scope.ServiceProvider.GetRequiredService<IConversationRepository>();

        var conversation = await conversationRepository.LoadAsync(conversationDomainMessage.UserId, conversationDomainMessage.ConversationId);

        conversation.UpdateTitle(conversationDomainMessage.Content);

        await conversationRepository.SaveAsync(conversation);

        await args.CompleteMessageAsync(args.Message);
    }

    private async Task OnMessageAsync(ProcessMessageEventArgs args)
    {
      

        var conversationDomainMessage = JsonSerializer.Deserialize<ConversationDomainMessage>(args.Message.Body, SerializerOptions);

        if (conversationDomainMessage == null)
        {
            throw new Exception("Failed to deserialize the Conversation Domain Message");
        }

        using var scope = _serviceProvider.CreateScope();
        
        var conversationRepository = scope.ServiceProvider.GetRequiredService<IConversationRepository>();

        var conversation = await conversationRepository.LoadAsync(conversationDomainMessage.UserId, conversationDomainMessage.ConversationId);

        conversation.CompleteConversationExchange(ExchangeId.FromGuid(conversationDomainMessage.ExchangeId), conversationDomainMessage.Content);

        await conversationRepository.SaveAsync(conversation);

        await args.CompleteMessageAsync(args.Message);
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