using Application.Dto;
using Application.Interfaces;
using Azure.Messaging.ServiceBus;
using Domain.Conversations;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Messaging;

public class ConversationWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ConversationWorker> _logger;
    private readonly ServiceBusProcessor _processor;

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
        _processor.ProcessMessageAsync += OnMessageAsync;
        _processor.ProcessErrorAsync += ErrorHandler;
    }

    private async Task OnMessageAsync(ProcessMessageEventArgs args)
    {
        await args.CompleteMessageAsync(args.Message);

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
    }

    private Task ErrorHandler(ProcessErrorEventArgs args)
    {
        _logger.LogError(args.Exception, "Conversation Message Worker Failed to Process Messages.");
        return Task.CompletedTask;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _processor.StartProcessingAsync(stoppingToken);
    }
}