using Agents.Conversation.Common;
using Agents.Infrastructure.Common;
using Agents.Infrastructure.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;

namespace Agents.Infrastructure.Extensions;

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