using ERP.Application.Abstractions.Messaging;
using ERP.Application.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace ERP.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddMediatR(config =>
            config.RegisterServicesFromAssembly(
                typeof(DependencyInjection).Assembly));

        services.AddScoped<
            IDomainEventDispatcher,
            DomainEventDispatcher>();

        return services;
    }
}