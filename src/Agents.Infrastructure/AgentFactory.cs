using Azure.AI.OpenAI;
using Microsoft.Agents.AI;
using Microsoft.SemanticKernel;
using OpenAI;
using System.ClientModel;
using Agents.Conversation;
using Microsoft.Extensions.Options;
using Agents.Conversation.Common;
using Agents.Infrastructure.Interfaces;
using Agents.Infrastructure.Settings;
using Microsoft.Extensions.Logging;

namespace Agents.Infrastructure;

public class AgentFactory(IAzureStorageRepository storageRepository, Kernel kernel, ILogger<AgentFactory> logger, IOptions<LanguageModelSettings> settings) : IAgentFactory
{
    private readonly LanguageModelSettings _settings = settings.Value;

    public async Task<IAgent> CreateAgent(string templateName)
    {
        string agentTemplate;
      
        try
        {
            agentTemplate = await storageRepository.DownloadTextBlobAsync(templateName, InfrastructureConstants.AgentTemplatesContainerName);

        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to load agent template : {ChatAgentTemplateName}", templateName);
            throw;
        }

        if (string.IsNullOrWhiteSpace(agentTemplate))
        {
            throw new InvalidOperationException($"The downloaded agent template is empty or invalid : {templateName}");
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