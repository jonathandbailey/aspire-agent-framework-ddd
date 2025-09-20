using Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace Infrastructure.Assistants;

/// <summary>
/// Factory class responsible for creating instances of <see cref="ChatCompletionAgent"/>.
/// </summary>
public class AssistantFactory(IAzureStorageRepository storageRepository, Kernel kernel, IStreamingEventPublisher publisher, ILogger<AssistantFactory> logger) : IAssistantFactory
{
    /// <summary>
    /// Creates an instance of <see cref="ChatCompletionAgent"/> using the agent template fetched from Azure Storage.
    /// </summary>
    /// <returns>A configured instance of <see cref="ChatCompletionAgent"/>.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the downloaded agent template is empty or invalid.</exception>
    /// <exception cref="Exception">Thrown when there is an error downloading the agent template.</exception>
    public async Task<IConversationAssistant> CreateConversationAssistant()
    {
        string agentTemplate;
        
        try
        {
            agentTemplate = await storageRepository.DownloadTextBlobAsync(InfrastructureConstants.ChatAgentTemplateName, InfrastructureConstants.AgentTemplatesContainerName);

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

        return new ConversationAssistant(agent, new DefaultMemoryStrategy(), publisher);
    }

    public async Task<IAssistant> CreateTitleAssistant()
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

        return new Assistant(agent);
    }
}