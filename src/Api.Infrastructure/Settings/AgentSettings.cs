namespace Api.Infrastructure.Settings;

public class AgentSettings
{
    public required Guid Id { get; init; }

    public required string TemplateName { get; init; }
}