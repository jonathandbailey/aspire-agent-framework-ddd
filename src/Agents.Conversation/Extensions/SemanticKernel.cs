using Agents.Conversation.Settings;
using Microsoft.SemanticKernel;

namespace Agents.Conversation.Extensions;

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

        serviceCollection.AddSingleton(_ => kernel);

        return serviceCollection;
    }
}