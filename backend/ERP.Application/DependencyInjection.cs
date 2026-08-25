
namespace ERP.Application;

using FluentValidation;

using ERP.Application.Abstractions.Messaging;
using ERP.Application.Behaviors;
using ERP.Application.Messaging;
using Microsoft.Extensions.DependencyInjection;

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
