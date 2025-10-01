using Api.Hub.Interfaces;
using Api.Hub.User;

namespace Api.Hub.Extensions;

public static  class ApiExtensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddSingleton<IUserConnectionManager, UserConnectionManager>();
 
        return services;
    }
}