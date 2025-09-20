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


var ui = builder.AddNpmApp(uiName, uiSourcePath, scriptName: uiScriptName)
    .WithReference(api)
    .WaitFor(api)
    .WithEnvironment(uiViteApiBaseUrl, api.GetEndpoint(uiEndPointReference))
    .WithHttpEndpoint(env: uiVitePort)
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

api.WithReference(ui);

var build = builder.Build();
build.Run();build.Run();