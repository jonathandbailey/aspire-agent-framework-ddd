using Agents.Infrastructure.Extensions;
using Agents.Infrastructure.Interfaces;
using Agents.Infrastructure.Services;
using Agents.Summarizer.Services;
using ServiceDefaults;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddSingleton<IAgentService, AgentService>();
builder.Services.AddHostedService<MessagingWorker>();

var host = builder.Build();
host.Run();
