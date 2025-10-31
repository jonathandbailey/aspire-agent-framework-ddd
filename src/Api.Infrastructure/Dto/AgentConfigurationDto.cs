namespace Api.Infrastructure.Dto;

public class AgentConfigurationDto
{
    public Guid Id { get; set; }

    public required string Template { get; init; }
}