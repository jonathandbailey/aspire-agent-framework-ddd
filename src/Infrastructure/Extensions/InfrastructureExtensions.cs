using Application.Interfaces;
using Infrastructure.Adapters;
using Infrastructure.Assistants;
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


        services.AddScoped<IAssistantFactory, AssistantFactory>();
        services.AddScoped<IConversationQueries, ConversationQuerieses>();

        services.AddAzureClients(azure =>
        {
            azure.AddBlobServiceClient(configuration.GetConnectionString(InfrastructureConstants.BlobStorageConnectionName));
        });

        services.Configure<AzureStorageSettings>((options)=> configuration.GetSection("AzureStorageSettings").Bind(options));

        var modelSettings = configuration.GetRequiredSetting<LanguageModelSettings>(InfrastructureConstants.LanguageModelSettingsKey);

        services.AddSemanticKernel(modelSettings);

        return services;
    }
}
