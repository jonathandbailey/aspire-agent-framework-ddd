using Agents.Conversation.Extensions;
using Agents.Conversation.Interfaces;
using Agents.Conversation.Services;
using Agents.Infrastructure.Extensions;
using Agents.Infrastructure.Interfaces;
using Agents.Infrastructure.Services;
using ServiceDefaults;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddSingleton<IAgentService, AgentService>();
builder.Services.AddSingleton<IConversationService, ConversationService>();

builder.Services.AddHostedService<MessagingWorker>();

var host = builder.Build();
host.Run();
