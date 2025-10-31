using Agents.Infrastructure.Common;
using Agents.Infrastructure.Interfaces;
using Agents.Infrastructure.Services;
using Agents.Infrastructure.Settings;
using Agents.Infrastructure.Storage;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Agents.Infrastructure.Extensions;

public static class InfrastructureExtensions
{
    private const string ApiInfrastructureHttpClientName = "ApiInfrastructure";

    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<LanguageModelSettings>((options) =>
            configuration.GetSection(InfrastructureConstants.LanguageModelSettingsKey).Bind(options));

        services.Configure<QueueSettings>(
            configuration.GetSection("Queues"));

        services.Configure<TopicSettings>(
            configuration.GetSection("Topics"));

        var modelSettings = configuration.GetRequiredSetting<LanguageModelSettings>(InfrastructureConstants.LanguageModelSettingsKey);

        services.AddSemanticKernel(modelSettings);

        services.AddAzureClients(azure =>
        {
            azure.AddServiceBusClient(configuration.GetConnectionString("messaging"));
            azure.AddBlobServiceClient(configuration.GetConnectionString(InfrastructureConstants.BlobStorageConnectionName));
        });

        // Register named HttpClient for API Infrastructure with service discovery
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
        services.AddSingleton<IAzureStorageRepository, AzureStorageRepository>();

        return services;
    }
}