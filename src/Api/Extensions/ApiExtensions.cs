using Api.Hubs;
using Api.Middleware;
using Application.Interfaces;

namespace Api.Extensions;

public static  class ApiExtensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddExceptionHandler<GlobalExceptionHandler>();
        
        services.AddScoped<IUserConnectionManager, UserConnectionManager>();
  
        services.AddScoped<IConversationClient, ConversationClient>();

        return services;
    }
}