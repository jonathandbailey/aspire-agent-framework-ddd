using Agents.Infrastructure;
using Agents.Infrastructure.Common;
using Agents.Infrastructure.Extensions;
using Agents.Infrastructure.Interfaces;
using Agents.Infrastructure.Services;
using Agents.Infrastructure.Settings;
using Microsoft.Extensions.Azure;

namespace Agents.Conversation.Extensions;

public static class InfrastructureExtensions
{
    private const string ApiInfrastructureHttpClientName = "ApiInfrastructure";

    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<LanguageModelSettings>((options) =>
            configuration.GetSection(InfrastructureConstants.LanguageModelSettingsKey).Bind(options));

        services.Configure<List<AgentSettings>>((options) => configuration.GetSection("AgentSettings").Bind(options));


        services.Configure<QueueSettings>(configuration.GetSection("Queues"));

        services.Configure<TopicSettings>(configuration.GetSection("Topics"));

        var modelSettings = configuration.GetRequiredSetting<LanguageModelSettings>(InfrastructureConstants.LanguageModelSettingsKey);

        services.AddSemanticKernel(modelSettings);

        services.AddAzureClients(azure =>
        {
            azure.AddServiceBusClient(configuration.GetConnectionString("messaging"));
            azure.AddBlobServiceClient(configuration.GetConnectionString(InfrastructureConstants.BlobStorageConnectionName));
        });

        services.AddHttpClient(ApiInfrastructureHttpClientName, (serviceProvider, client) =>
        {
            var apiInfraSettings = configuration["services:api-infrastructure:http:0"]
                ?? configuration["services:api-infrastructure:https:0"];
            
            if (!string.IsNullOrWhiteSpace(apiInfraSettings))
            {
                client.BaseAddress = new Uri(apiInfraSettings);
            }
        });

        services.AddSingleton<IAgentFactory, AgentFactory>();
        services.AddSingleton<IAgentDataService, AgentDataService>();
  
        return services;
    }
}