using Agents.Conversation.Common;
using Agents.Conversation.Interfaces;
using Agents.Conversation.Workflows;
using Agents.Infrastructure.Dto;
using Agents.Infrastructure.Extensions;
using Agents.Infrastructure.Interfaces;
using Agents.Infrastructure.Settings;
using Azure;
using Azure.Messaging.ServiceBus;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Agents.Conversation.Services;

public class AgentService(ServiceBusClient serviceBusClient, IAgentFactory agentFactory, IConversationService conversationService, IOptions<TopicSettings> settings)
    : IAgentService
{
    private readonly ServiceBusSender _userStreamSender = serviceBusClient.CreateSender(settings.Value.User);

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

    private async Task ProcessAsync(ConversationAgentMessage agentConversationRequest)
    {
        var agent = await agentFactory.CreateAgent(InfrastructureConstants.ChatAgentTemplateName);

        var stringBuilder = new StringBuilder();

        var messages = new List<ChatMessage>();

        foreach (var message in  agentConversationRequest.Messages)
        {
            if (message.Role == "user")
            {
                var userChatMessage = new ChatMessage(ChatRole.User, message.Content);

                messages.Add(userChatMessage);
            }

            if (message.Role == "assistant")
            {
                var userChatMessage = new ChatMessage(ChatRole.Assistant, message.Content);

                messages.Add(userChatMessage);
            }
        }

        var workflowBuilder = new WorkflowBuilder(new ConversationNode(agent));

        var workflow = await workflowBuilder.BuildAsync<List<ChatMessage>>();

        var run = await InProcessExecution.StreamAsync(workflow, messages);

        await run.TrySendMessageAsync(new TurnToken(emitEvents: true));
       
        await foreach (var evt in run.WatchStreamAsync())
        {
            if (evt is ConversationStreamingEvent { Data: not null } streamingEvent)
            {
                stringBuilder.Append(streamingEvent.Data);

                var messageString = streamingEvent.Data?.ToString() ?? string.Empty;
                var payload = new ConversationStreamingMessage(agentConversationRequest.UserId, messageString,
                    agentConversationRequest.ConversationId, agentConversationRequest.ExchangeId);

                var serializedConversation = JsonSerializer.Serialize(payload, SerializerOptions);

                var serviceBusMessage = new ServiceBusMessage(serializedConversation)
                {
                    ApplicationProperties =
                    {
                        { "Target" , "UserConversationStream"}
                    }
                };

                await _userStreamSender.SendMessageAsync(serviceBusMessage);
            }
        }

        await conversationService.PublishDomainUpdate(agentConversationRequest.UserId, stringBuilder.ToString(),
            agentConversationRequest.ConversationId, agentConversationRequest.ExchangeId);
    }
}