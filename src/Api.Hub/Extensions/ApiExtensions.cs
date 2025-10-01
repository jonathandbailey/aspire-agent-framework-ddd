using Api.Hub.Interfaces;
using Api.Hub.Services;
using Api.Hub.User;

namespace Api.Hub.Extensions;

public static  class ApiExtensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddHostedService<ConversationMessageWorker>();

        services.AddSingleton<IUserConnectionManager, UserConnectionManager>();

        services.AddSingleton<IMessageRoutingService, MessageRoutingService>();
 
        return services;
    }
}