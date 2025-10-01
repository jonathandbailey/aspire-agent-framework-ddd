using Api.Middleware;

namespace Api.Extensions;

public static  class ApiExtensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddExceptionHandler<GlobalExceptionHandler>();
    
        return services;
    }
}