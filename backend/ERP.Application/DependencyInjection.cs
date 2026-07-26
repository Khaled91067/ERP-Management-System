using ERP.Application.Abstractions.Messaging;
using ERP.Application.Messaging;
using ERP.Application.Behaviors;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace ERP.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
            config.AddOpenBehavior(typeof(LoggingBehavior<,>));
            config.AddOpenBehavior(typeof(PerformanceBehavior<,>));
            config.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddScoped<
            IDomainEventDispatcher,
            DomainEventDispatcher>();

        return services;
    }
}