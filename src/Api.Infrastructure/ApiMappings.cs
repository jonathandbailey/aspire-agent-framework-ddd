using Api.Infrastructure.Dto;
using Api.Infrastructure.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Api.Infrastructure;

public static class ApiMappings
{
    private const string ApiRoot = "api";
    private const string GetAgentTemplate = "/agents/templates/{templateId:guid}";

    public static WebApplication MapApi(this WebApplication app)
    {
        var api = app.MapGroup(ApiRoot);
  
        api.MapGet(GetAgentTemplate, ConversationSummaries);
   
        return app;
    }

    private static async Task<Ok<AgentConfigurationDto>> ConversationSummaries(IAgentTemplateService agentTemplateService, Guid templateId)
    {
        var configuration = await agentTemplateService.GetAgentConfiguration(templateId);

        return TypedResults.Ok(configuration);
    }
}