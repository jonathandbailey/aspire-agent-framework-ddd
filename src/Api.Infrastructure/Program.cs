using Api.Infrastructure;
using Api.Infrastructure.Interfaces;
using Api.Infrastructure.Services;
using Api.Infrastructure.Settings;
using Microsoft.Extensions.Azure;
using ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IAzureStorageRepository, AzureStorageRepository>();
builder.Services.AddScoped<IAgentTemplateService, AgentTemplateService>();

builder.Services.AddHostedService<SeedService>();

builder.Services.Configure<List<AgentSettings>>((options) => builder.Configuration.GetSection("AgentSettings").Bind(options));

builder.Services.AddAzureClients(azure =>
{
    azure.AddBlobServiceClient(builder.Configuration.GetConnectionString("blobs"));
});

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapApi();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHttpsRedirection();
}
   

app.Run();

