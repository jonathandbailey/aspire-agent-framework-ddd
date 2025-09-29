using Hub;
using Hub.Extensions;
using ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddOpenApi();

builder.Services.AddApiServices();

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

app.MapHub<ChatHub>("hub");

app.UseCorsPolicyServiceDiscovery();


app.Run();
