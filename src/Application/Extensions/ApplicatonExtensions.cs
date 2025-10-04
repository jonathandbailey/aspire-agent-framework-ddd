using Application.Behaviours;
using Application.Interfaces;
using Application.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Application.Messaging;
using Domain.Interfaces;
using Domain.Services;

namespace Application.Extensions;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(typeof(ApplicationExtensions).Assembly);
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddScoped<IStreamingEventPublisher, StreamingEventPublisher>();

        services.AddScoped<IConversationDomainService, ConversationDomainService>();

        services.AddHostedService<ConversationWorker>();


        return services;
    }
}