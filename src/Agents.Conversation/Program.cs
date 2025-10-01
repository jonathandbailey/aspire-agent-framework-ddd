using Agents.Conversation;
using Agents.Conversation.Extensions;
using Agents.Conversation.Settings;
using Agents.Conversation.Storage;
using Application.Interfaces;
using Microsoft.Extensions.Azure;
using ServiceDefaults;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddAzureClients(azure =>
{
    azure.AddServiceBusClient(builder.Configuration.GetConnectionString("messaging"));
    azure.AddBlobServiceClient(builder.Configuration.GetConnectionString(InfrastructureConstants.BlobStorageConnectionName));
});

builder.AddServiceDefaults();
builder.Services.AddSingleton<IAgentFactory, AgentFactory>();
builder.Services.AddSingleton<IAzureStorageRepository, AzureStorageRepository>();

var modelSettings = builder.Configuration.GetRequiredSetting<LanguageModelSettings>(InfrastructureConstants.LanguageModelSettingsKey);

builder.Services.AddSemanticKernel(modelSettings);

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
