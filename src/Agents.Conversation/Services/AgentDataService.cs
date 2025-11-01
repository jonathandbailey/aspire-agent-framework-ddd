using System.Net.Http.Json;
using Agents.Infrastructure.Dto;
using Agents.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;

namespace Agents.Infrastructure.Services;

public class AgentDataService(
    IHttpClientFactory httpClientFactory,
    ILogger<AgentDataService> logger) : IAgentDataService
{
    private const string ApiInfrastructureHttpClientName = "ApiInfrastructure";

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