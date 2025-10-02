using Agents.Conversation.Dto;
using Application.Interfaces;
using Azure.Messaging.ServiceBus;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Agents.Conversation;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IAgentFactory _agentFactory;
    private readonly ServiceBusProcessor _processor;
    private readonly ServiceBusSender _sender;

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter() }
    };

    public Worker(ServiceBusClient serviceBusClient, ILogger<Worker> logger, IAgentFactory agentFactory)
    {
        _logger = logger;
        _agentFactory = agentFactory;
        _processor = serviceBusClient.CreateProcessor("queue", new ServiceBusProcessorOptions());
        _processor.ProcessMessageAsync += OnMessageAsync;
        _processor.ProcessErrorAsync += ErrorHandler;

        _sender = serviceBusClient.CreateSender("topic");
    }

    private async Task OnMessageAsync(ProcessMessageEventArgs args)
    {
        var conversation = JsonSerializer.Deserialize<ConversationAgentMessage>(args.Message.Body, SerializerOptions);

        if (conversation == null)
        {
            throw new Exception("Failed to deserialize conversation.");
        }

        var agent = await _agentFactory.CreateAgent("Conversation");

        await foreach (var response in agent.InvokeStreamAsync(conversation.Messages))
        {
            var payload = new ConversationStreamingMessage(conversation.UserId, response.Content,
                conversation.ConversationId, conversation.ExchangeId);
            
            var serializedConversation = JsonSerializer.Serialize(payload, SerializerOptions);

            var serviceBusMessage = new ServiceBusMessage(serializedConversation)
            {
                ApplicationProperties =
                {
                    { "Target" , "UserConversationStream"}
                }
            };

            await _sender.SendMessageAsync(serviceBusMessage);
        }

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
