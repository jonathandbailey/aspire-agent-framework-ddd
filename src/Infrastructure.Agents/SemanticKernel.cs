using Infrastructure.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;

namespace Infrastructure.Agents;

public static class SemanticKernel
{
    public static IServiceCollection AddSemanticKernel(this IServiceCollection serviceCollection, LanguageModelSettings settings)
    {
        var kernelBuilder = Kernel.CreateBuilder();

        kernelBuilder.AddAzureOpenAIChatCompletion(
            deploymentName: settings.DeploymentName,
            apiKey: settings.ApiKey,
            endpoint: settings.EndPoint,
            serviceId: InfrastructureConstants.ChatAgentModeServiceId
        );

        var kernel = kernelBuilder.Build();

        serviceCollection.AddScoped(_ => kernel);

        return serviceCollection;
    }
}