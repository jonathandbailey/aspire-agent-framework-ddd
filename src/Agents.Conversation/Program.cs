using Agents.Conversation;
using Agents.Conversation.Common;
using Agents.Conversation.Extensions;
using Agents.Conversation.Interfaces;
using Agents.Conversation.Services;
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
builder.Services.AddSingleton<IAgentService, AgentService>();
builder.Services.AddSingleton<IConversationService, ConversationService>();

builder.Services.Configure<LanguageModelSettings>((options) => 
    builder.Configuration.GetSection(InfrastructureConstants.LanguageModelSettingsKey).Bind(options));


var modelSettings = builder.Configuration.GetRequiredSetting<LanguageModelSettings>(InfrastructureConstants.LanguageModelSettingsKey);

builder.Services.AddSemanticKernel(modelSettings);

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
