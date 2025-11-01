using Azure.AI.OpenAI;
using Microsoft.Agents.AI;
using Microsoft.SemanticKernel;
using OpenAI;
using System.ClientModel;
using Microsoft.Extensions.Options;
using Agents.Infrastructure.Dto;
using Agents.Infrastructure.Interfaces;
using Agents.Infrastructure.Settings;
using Microsoft.Extensions.Logging;


namespace Agents.Infrastructure;

public class AgentFactory(IAgentDataService agentDataService, Kernel kernel, ILogger<AgentFactory> logger, IOptions<LanguageModelSettings> settings) : IAgentFactory
{
    private readonly LanguageModelSettings _settings = settings.Value;

    public async Task<AIAgent> CreateAgent(Guid id)
    {
        AgentConfigurationDto agentConfiguration;
      
        try
        {
            agentConfiguration = await agentDataService.GetAgentConfigurationAsync(id);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to load agent template");
            throw;
        }

        if (string.IsNullOrWhiteSpace(agentConfiguration.Template))
        {
            throw new InvalidOperationException($"The downloaded agent template is empty or invalid");
        }

        var factory = new KernelPromptTemplateFactory();

        var promptTemplate = factory.Create(new PromptTemplateConfig(agentConfiguration.Template));

        var kernelArguments = new KernelArguments();
        var rendered = await promptTemplate.RenderAsync(kernel, kernelArguments);

        var chatClient = new AzureOpenAIClient(new Uri(_settings.EndPoint),
                new ApiKeyCredential(
                    _settings.ApiKey))
            .GetChatClient(_settings.DeploymentName);
    
        var agent = chatClient.CreateAIAgent(new ChatClientAgentOptions
        {
            Instructions = rendered,
         
            AIContextProviderFactory = _ => new ChatHistoryContextProvider()
        });

        return agent;
    }
}