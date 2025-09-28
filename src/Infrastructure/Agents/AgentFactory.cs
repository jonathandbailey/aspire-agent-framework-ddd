using Application.Interfaces;
using Infrastructure.Assistants;
using Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Connectors.OpenAI;


namespace Infrastructure.Agents;

/// <summary>
/// Factory class responsible for creating instances of <see cref="ChatCompletionAgent"/>.
/// </summary>
public class AgentFactory(IAzureStorageRepository storageRepository, Kernel kernel, ILogger<AgentFactory> logger) : IAgentFactory
{

    private readonly List<AgentSettings> _agentSettings = [ new AgentSettings { Name = "Conversation", PromptTemplateName = "chat-assistant.yaml" }];
    
    
    /// <summary>
    /// Creates an instance of <see cref="ChatCompletionAgent"/> using the agent template fetched from Azure Storage.
    /// </summary>
    /// <returns>A configured instance of <see cref="ChatCompletionAgent"/>.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the downloaded agent template is empty or invalid.</exception>
    /// <exception cref="Exception">Thrown when there is an error downloading the agent template.</exception>
    public async Task<IAgent> CreateAgent(string name)
    {
        string agentTemplate;

        var agentSettings = _agentSettings.First(x => x.Name == name);
        
        try
        {
            agentTemplate = await storageRepository.DownloadTextBlobAsync(agentSettings.PromptTemplateName, InfrastructureConstants.AgentTemplatesContainerName);

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

        var templateConfig = KernelFunctionYaml.ToPromptTemplateConfig(agentTemplate);

        var promptExecutionSettings = new OpenAIPromptExecutionSettings
        {
            ServiceId = InfrastructureConstants.ChatAgentModeServiceId
        };

        var agent = new ChatCompletionAgent(templateConfig, new KernelPromptTemplateFactory())
        {
            Kernel = kernel,
            Arguments = new KernelArguments(promptExecutionSettings)
        };

        var streamingAgent = new BaseStreamingAgent(agent);

        return new Agent(streamingAgent);
    }

    public async Task<ITitleAssistant> CreateTitleAssistant()
    {
        string agentTemplate;

        try
        {
            agentTemplate = await storageRepository.DownloadTextBlobAsync(InfrastructureConstants.TitleAssistantName, InfrastructureConstants.AgentTemplatesContainerName);

        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to load agent template : {ChatAgentTemplateName}", InfrastructureConstants.TitleAssistantName);
            throw;
        }

        if (string.IsNullOrWhiteSpace(agentTemplate))
        {
            throw new InvalidOperationException($"The downloaded agent template is empty or invalid : {InfrastructureConstants.TitleAssistantName}");
        }

        var templateConfig = KernelFunctionYaml.ToPromptTemplateConfig(agentTemplate);

        var promptExecutionSettings = new OpenAIPromptExecutionSettings
        {
            ServiceId = InfrastructureConstants.ChatAgentModeServiceId
        };

        var agent = new ChatCompletionAgent(templateConfig, new KernelPromptTemplateFactory())
        {
            Kernel = kernel,
            Arguments = new KernelArguments(promptExecutionSettings)
        };

        return new TitleAssistant(agent);
    }
}