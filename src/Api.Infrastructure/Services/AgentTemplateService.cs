using Api.Infrastructure.Dto;
using Api.Infrastructure.Interfaces;
using Api.Infrastructure.Settings;
using Microsoft.Extensions.Options;

namespace Api.Infrastructure.Services;

public class AgentTemplateService(IAzureStorageRepository azureStorageRepository, IOptions<List<AgentSettings>> settings) : IAgentTemplateService
{
    public async Task<AgentConfigurationDto> GetAgentConfiguration(Guid id)
    {
        var config = settings.Value.First(x => x.Id == id);

        var template = await azureStorageRepository.DownloadTextBlobAsync(config.TemplateName, "agent-templates");

        return new AgentConfigurationDto() { Id = id, Template = template };
    }
}