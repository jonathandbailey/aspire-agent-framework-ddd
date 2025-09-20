using Api.Hubs;
using Application.Interfaces;

namespace Api.Extensions;

public static  class ApiExtensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddScoped<IUserConnectionManager, UserConnectionManager>();
  
        services.AddScoped<IConversationClient, ConversationClient>();

        return services;
    }
}