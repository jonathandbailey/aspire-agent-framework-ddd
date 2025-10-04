namespace AppHost.Extensions;

public static class UiServicesExtensions
{
    private const string UiName = "ui";
    private const string UiSourcePath = "../../ui";
    private const string UiScriptName = "dev";
    private const string UiViteApiBaseUrl = "VITE_API_BASE_URL";
    private const string UiVitePort = "VITE_PORT";
    private const string UiEndPointReference = "http";

    public static IResourceBuilder<NodeAppResource> AddUiServices(this IDistributedApplicationBuilder builder, IResourceBuilder<ProjectResource> api, IResourceBuilder<ProjectResource> hub)
    {
        var ui = builder.AddNpmApp(UiName, UiSourcePath, scriptName: UiScriptName)
            .WithReference(api)
            .WithReference(hub)
            .WaitFor(api)
            .WaitFor(hub)
            .WithEnvironment(UiViteApiBaseUrl, api.GetEndpoint(UiEndPointReference))
            .WithEnvironment("VITE_HUB_BASE_URL", hub.GetEndpoint(UiEndPointReference))
            .WithHttpEndpoint(env: UiVitePort)
            .WithExternalHttpEndpoints()
            .PublishAsDockerFile();

        return ui;
    }
}