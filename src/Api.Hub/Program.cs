using Api.Hub.Extensions;
using Api.Hub.User;
using Hub.Extensions;
using Microsoft.Extensions.Azure;
using ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddOpenApi();

builder.Services.AddApiServices();

builder.Services.AddAzureClients(azure =>
{
    azure.AddServiceBusClient(builder.Configuration.GetConnectionString("messaging"));
});

builder.Services.AddSignalR();

builder.AddCorsPolicyFromServiceDiscovery();

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
else
{
    app.UseHttpsRedirection();
}

app.MapHub<UserHub>("hub");

app.UseCorsPolicyServiceDiscovery();

app.Run();