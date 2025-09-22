using Application.Interfaces;
using Infrastructure.Adapters;
using Infrastructure.Assistants;
using Infrastructure.Interfaces;
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


        services.AddScoped<IAssistantMemory, DefaultMemoryStrategy>();
        services.AddScoped<IAssistantFactory, AssistantFactory>();
        services.AddScoped<IConversationQuery, ConversationQuery>();

        services.AddAzureClients(azure =>
        {
            azure.AddBlobServiceClient(configuration.GetConnectionString(InfrastructureConstants.BlobStorageConnectionName));
        });

        var modelSettings = configuration.GetRequiredSetting<LanguageModelSettings>(InfrastructureConstants.LanguageModelSettingsKey);

        services.AddSemanticKernel(modelSettings);

        return services;
    }
}
