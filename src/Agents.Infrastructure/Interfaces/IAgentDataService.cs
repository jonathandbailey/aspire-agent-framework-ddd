using Agents.Infrastructure.Dto;

namespace Agents.Infrastructure.Interfaces;

public interface IAgentDataService
{
    Task<AgentConfigurationDto> GetAgentConfiguration(Guid id, string templateName);
    Task<AgentConfigurationDto> GetAgentConfigurationAsync(Guid id);
}