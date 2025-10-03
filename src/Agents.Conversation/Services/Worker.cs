using Agents.Conversation.Dto;
using Azure.Messaging.ServiceBus;
using System.Text.Json;
using System.Text.Json.Serialization;
using Agents.Conversation.Interfaces;

namespace Agents.Conversation.Services;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IAgentService _agentService;
    private readonly IConversationService _conversationService;
    private readonly ServiceBusProcessor _processor;

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter() }
    };

    public Worker(ServiceBusClient serviceBusClient, ILogger<Worker> logger, IAgentService agentService, IConversationService conversationService)
    {
        _logger = logger;
        _agentService = agentService;
        _conversationService = conversationService;
        _processor = serviceBusClient.CreateProcessor("queue", new ServiceBusProcessorOptions());
        _processor.ProcessMessageAsync += OnMessageAsync;
        _processor.ProcessErrorAsync += ErrorHandler;
    }

    private async Task OnMessageAsync(ProcessMessageEventArgs args)
    {
        var agentConversationRequest = JsonSerializer.Deserialize<ConversationAgentMessage>(args.Message.Body, SerializerOptions);

        if (agentConversationRequest == null)
        {
            throw new Exception("Failed to deserialize conversation.");
        }

        var agentResponse = await _agentService.ProcessAsync(agentConversationRequest);

        await _conversationService.PublishDomainUpdate(agentConversationRequest.UserId, agentResponse.Content,
            agentConversationRequest.ConversationId, agentConversationRequest.ExchangeId);

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
