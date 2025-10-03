using Agents.Conversation.Settings;
using Application.Interfaces;
using Azure.AI.OpenAI;
using Microsoft.Agents.AI;
using Microsoft.SemanticKernel;
using OpenAI;
using System.ClientModel;
using Agents.Conversation.Interfaces;
using Microsoft.Extensions.Options;
using Agents.Conversation.Common;

namespace Agents.Conversation;

public class AgentFactory(IAzureStorageRepository storageRepository, Kernel kernel, ILogger<AgentFactory> logger, IOptions<LanguageModelSettings> settings) : IAgentFactory
{
    private readonly LanguageModelSettings _settings = settings.Value;

    public async Task<IAgent> CreateAgent()
    {
        string agentTemplate;
      
        try
        {
            agentTemplate = await storageRepository.DownloadTextBlobAsync("chat-assistant.yaml", InfrastructureConstants.AgentTemplatesContainerName);

        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to load agent template : {ChatAgentTemplateName}", InfrastructureConstants.ChatAgentTemplateName);
            throw;
        }

        if (string.IsNullOrWhiteSpace(agentTemplate))
        {
            throw new InvalidOperationException($"The downloaded agent template is empty or invalid : {InfrastructureConstants.ChatAgentTemplateName}");
        }

        var factory = new KernelPromptTemplateFactory();

        var promptTemplate = factory.Create(new PromptTemplateConfig(agentTemplate));

        var rendered = await promptTemplate.RenderAsync(kernel);

        var chatClient = new AzureOpenAIClient(new Uri(_settings.EndPoint),
                new ApiKeyCredential(
                    _settings.ApiKey))
            .GetChatClient(_settings.DeploymentName);
    
        var agent = chatClient.CreateAIAgent(new ChatClientAgentOptions
        {
            Instructions = rendered,

            ChatMessageStoreFactory = ctx =>
                new InMemoryChatMessageStore(ctx.SerializedState, ctx.JsonSerializerOptions)
        });

        return new Agent(agent);
    }
}