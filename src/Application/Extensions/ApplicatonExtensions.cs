using Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Extensions;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(typeof(ApplicationExtensions).Assembly);
        });

        services.AddScoped<IStreamingEventPublisher, StreamingEventPublisher>();

        return services;
    }
}