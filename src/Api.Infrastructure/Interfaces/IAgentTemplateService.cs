using Api.Infrastructure.Dto;

namespace Api.Infrastructure.Interfaces;

public interface IAgentTemplateService
{
    Task<AgentConfigurationDto> GetAgentConfiguration(Guid id);
}