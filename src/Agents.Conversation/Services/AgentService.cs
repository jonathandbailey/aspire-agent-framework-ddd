using Agents.Conversation.Common;
using Agents.Conversation.Interfaces;
using Agents.Conversation.State;
using Agents.Conversation.Workflows;
using Agents.Infrastructure.Dto;
using Agents.Infrastructure.Interfaces;
using Azure.Messaging.ServiceBus;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Agents.Conversation.Extensions;

namespace Agents.Conversation.Services;

public class AgentService(IAgentFactory agentFactory, IConversationService conversationService) : IAgentService
{
  
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter() }
    };

    public async Task ProcessAsync(ServiceBusReceivedMessage message)
    {
        var agentConversationRequest = JsonSerializer.Deserialize<ConversationAgentMessage>(message.Body, SerializerOptions);

        if (agentConversationRequest == null)
        {
            throw new Exception("Failed to deserialize conversation.");
        }

        await ProcessAsync(agentConversationRequest);
    }

    private async Task ProcessAsync(ConversationAgentMessage request)
    {
        var agent = await agentFactory.CreateAgent(InfrastructureConstants.ChatAgentTemplateName);

        var stringBuilder = new StringBuilder();

        var messages = request.Messages.Map();

        var conversationNode = new ConversationNode(agent);
        var domainNode = new ConversationDomainNode(conversationService);
        
        var builder = new WorkflowBuilder(conversationNode);

        builder.AddEdge(conversationNode, domainNode);

        var workflow = await builder.BuildAsync<ConversationState>();

        var state = new ConversationState(messages, request.UserId, request.ConversationId, request.ExchangeId,
            string.Empty);

        var run = await InProcessExecution.StreamAsync(workflow, state);

        await run.TrySendMessageAsync(new TurnToken(emitEvents: true));
       
        await foreach (var evt in run.WatchStreamAsync())
        {
            if (evt is ConversationStreamingEvent { Data: not null } streamingEvent)
            {
                stringBuilder.Append(streamingEvent.Data);

                var messageString = streamingEvent.Data?.ToString() ?? string.Empty;

                await conversationService.PublishUserStream(request.UserId, messageString,
                    request.ConversationId, request.ExchangeId);
            }
        }
    }
}