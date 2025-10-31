using Agents.Infrastructure.Common;
using Agents.Infrastructure.Dto;
using Agents.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;

namespace Agents.Infrastructure.Services;

public class AgentDataService(IAzureStorageRepository storageRepository, ILogger<AgentDataService> logger) : IAgentDataService
{
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
}