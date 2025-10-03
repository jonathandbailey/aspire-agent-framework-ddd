using Agents.Conversation.Common;
using Agents.Infrastructure.Interfaces;
using Agents.Infrastructure.Settings;
using Agents.Infrastructure.Storage;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Agents.Infrastructure.Extensions;

public static  class InfrastructureExtensions
{
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

        services.AddSingleton<IAgentFactory, AgentFactory>();
        services.AddSingleton<IAzureStorageRepository, AzureStorageRepository>();

        return services;
    }
}