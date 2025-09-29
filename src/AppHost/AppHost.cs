var builder = DistributedApplication.CreateBuilder(args);

const string apiName = "api";
const string uiName = "ui";
const string uiSourcePath = "../../ui"; 
const string uiScriptName = "dev";
const string uiViteApiBaseUrl = "VITE_API_BASE_URL";
const string uiVitePort = "VITE_PORT";
const string uiEndPointReference = "http";

const string storageName = "storage";
const string storageData = "../Storage/Data";
const string blobStorageConnectionName = "blobs";

var storage = builder.AddAzureStorage(storageName)
    .RunAsEmulator(resourceBuilder =>
        { resourceBuilder.WithDataBindMount(storageData); });

var blobs = storage.AddBlobs(blobStorageConnectionName);

var api = builder.AddProject<Projects.Api>(apiName).WithReference(blobs).WaitFor(storage);

var hub = builder.AddProject<Projects.Hub>("hub");

var ui = builder.AddNpmApp(uiName, uiSourcePath, scriptName: uiScriptName)
    .WithReference(api)
    .WithReference(hub)
    .WaitFor(api)
    .WaitFor(hub)
    .WithEnvironment(uiViteApiBaseUrl, api.GetEndpoint(uiEndPointReference))
    .WithEnvironment("VITE_HUB_BASE_URL", hub.GetEndpoint(uiEndPointReference))
    .WithHttpEndpoint(env: uiVitePort)
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

api.WithReference(ui);


var build = builder.Build();
build.Run();