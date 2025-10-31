using System.Net.Http.Json;
using Agents.Infrastructure.Common;
using Agents.Infrastructure.Dto;
using Agents.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;

namespace Agents.Infrastructure.Services;

public class AgentDataService(
    IAzureStorageRepository storageRepository,
    IHttpClientFactory httpClientFactory,
    ILogger<AgentDataService> logger) : IAgentDataService
{
    private const string ApiInfrastructureHttpClientName = "ApiInfrastructure";

    public async Task<AgentConfigurationDto> GetAgentConfiguration(Guid id, string templateName)
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

        return new AgentConfigurationDto() { Id = id, Template = agentTemplate };
    }

    public async Task<AgentConfigurationDto> GetAgentConfigurationAsync(Guid id)
    {
        try
        {
            var httpClient = httpClientFactory.CreateClient(ApiInfrastructureHttpClientName);
            var response = await httpClient.GetAsync($"api/agents/templates/{id}");

            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning("Failed to get agent configuration for ID {Id}. Status: {StatusCode}", id, response.StatusCode);
                throw new Exception();
            }

            var agentConfiguration = await response.Content.ReadFromJsonAsync<AgentConfigurationDto>();
            return agentConfiguration ?? throw new InvalidOperationException();
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to get agent configuration for ID: {Id}", id);
            throw;
        }
    }
}