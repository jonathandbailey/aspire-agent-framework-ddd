using Api.Infrastructure.Dto;

namespace Api.Infrastructure.Services;

public interface IAgentTemplateService
{
    Task<AgentConfigurationDto> GetAgentConfiguration(Guid id);
}