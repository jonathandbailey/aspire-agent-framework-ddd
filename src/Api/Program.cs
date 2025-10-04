using Api;
using Api.Extensions;
using Api.Middleware;
using Application.Extensions;
using Infrastructure.Agents.Extensions;
using Infrastructure.Extensions;
using ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.AddCorsPolicyFromServiceDiscovery();

builder.Services.AddApiServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddAgentInfrastructureServices(builder.Configuration);

builder.Services.AddApplicationServices();

builder.Services.AddTransient<GlobalExceptionHandler>();

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapConversationApi();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHttpsRedirection();
}

app.UseExceptionHandler(options => { });

app.UseCorsPolicyServiceDiscovery();

app.UseAuthorization();

app.MapControllers();


app.Run();