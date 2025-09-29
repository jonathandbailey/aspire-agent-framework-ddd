using Application.Interfaces;

namespace Hub.Extensions;

public static  class ApiExtensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddScoped<IUserConnectionManager, UserConnectionManager>();
 
        return services;
    }
}