using Application.Interfaces;
using Infrastructure.Adapters;
using Infrastructure.Messaging;
using Infrastructure.Queries;
using Infrastructure.Settings;
using Infrastructure.Storage;
using MediatR;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IAzureStorageRepository, AzureStorageRepository>();
        services.AddScoped<ConversationRepository>();

        services.AddScoped<IConversationRepository>(sp =>
        {
            var repository = sp.GetRequiredService<ConversationRepository>();
            var mediator = sp.GetRequiredService<IMediator>();

            return new ConversationRepositoryDomainAdapter(mediator, repository);
        });
 
        services.AddScoped<IConversationQueries, ConversationQuerieses>();

        services.AddAzureClients(azure =>
        {
            azure.AddBlobServiceClient(configuration.GetConnectionString(InfrastructureConstants.BlobStorageConnectionName));
            azure.AddServiceBusClient( configuration.GetConnectionString("messaging"));
        });

        services.AddScoped<IIntegrationMessaging, AzureMessageBus>();

        services.AddHostedService<IntegrationMessageWorker>();

        services.Configure<AzureStorageSettings>((options)=> configuration.GetSection("AzureStorageSettings").Bind(options));

        services.Configure<QueueSettings>(configuration.GetSection("Queues"));
        services.Configure<TopicSettings>(configuration.GetSection("Topics"));

        return services;
    }
}
