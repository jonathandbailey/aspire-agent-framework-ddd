using Agents.Infrastructure.Dto;

namespace Agents.Infrastructure.Interfaces;

public interface IAgentDataService
{
    Task<AgentConfigurationDto> GetAgentConfigurationAsync(Guid id);
}