using Application.Interfaces;
using Infrastructure.Extensions;
using Infrastructure.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Agents.Extensions;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddAgentInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IAgentFactory, AgentFactory>();
       
        var modelSettings = configuration.GetRequiredSetting<LanguageModelSettings>(InfrastructureConstants.LanguageModelSettingsKey);

        services.AddSemanticKernel(modelSettings);

        return services;
    }
}
