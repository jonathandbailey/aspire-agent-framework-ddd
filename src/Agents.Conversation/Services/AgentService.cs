using Agents.Conversation.Interfaces;
using Agents.Conversation.State;
using Agents.Conversation.Workflows;
using Agents.Infrastructure.Dto;
using Agents.Infrastructure.Interfaces;
using Azure.Messaging.ServiceBus;
using Microsoft.Agents.AI.Workflows;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Agents.Conversation.Extensions;
using Agents.Conversation.Workflows.ReAct;

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

        await ReActWorkflow(agentConversationRequest);
    }

    private async Task ReActWorkflow(ConversationAgentMessage request)
    {
        var conversationAgent = await agentFactory.CreateConversationReasonAgent();
        
        var messages = request.Messages.Map();

        var conversationNode = new ConversationReasonNode(conversationAgent);
  
        var builder = new WorkflowBuilder(conversationNode);

        var workflow = await builder.BuildAsync<ConversationState>();

        var state = new ConversationState(messages,
            request.UserId,
            request.ConversationId,
            request.ExchangeId,
            request.Title,
            string.Empty);

        var run = await InProcessExecution.StreamAsync(workflow, state);

        await run.TrySendMessageAsync(new TurnToken(emitEvents: true));

        await foreach (var evt in run.WatchStreamAsync())
        {
            if (evt is ConversationStreamingEvent { Data: not null } streamingEvent)
            {
               
            }
        }
    }

    private async Task DefaultWorkflow(ConversationAgentMessage request)
    {
        var conversationAgent = await agentFactory.CreateConversationAgent();
        var summarizerAgent = await agentFactory.CreateTitleAgent();

        var stringBuilder = new StringBuilder();

        var messages = request.Messages.Map();

        var conversationNode = new ConversationNode(conversationAgent);
        var domainNode = new ConversationDomainNode(conversationService);
        var titleNode = new TitleNode(summarizerAgent, conversationService);

        var builder = new WorkflowBuilder(conversationNode);

        builder.AddEdge(conversationNode, domainNode);
        builder.AddEdge<ConversationState>(source: conversationNode, target: titleNode, condition: arg =>

            string.IsNullOrEmpty(arg?.Title)
        );

        var workflow = await builder.BuildAsync<ConversationState>();

        var state = new ConversationState(messages,
            request.UserId,
            request.ConversationId,
            request.ExchangeId,
            request.Title,
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