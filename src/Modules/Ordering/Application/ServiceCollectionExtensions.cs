using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Template.Core.Shared.Kernel.Communication;

namespace Template.Core.Ordering.Application;

public static class ServiceCollectionExtensions
{
    public static void AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        _ = configuration;
        services.AddScoped<IMediator, Mediator>();
        services.AddScoped<IPipelineBehavior, ValidationBehavior>();

        var types = typeof(ServiceCollectionExtensions).Assembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false })
            .ToList();

        // Every command/query handler is registered as the closed IRequestHandler<,> the mediator resolves.
        foreach (var type in types)
        {
            foreach (var service in type.GetInterfaces()
                         .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>)))
            {
                services.AddScoped(service, type);
            }
        }

        // Every FluentValidation validator is picked up by ValidationBehavior.
        foreach (var type in types)
        {
            foreach (var service in type.GetInterfaces()
                         .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IValidator<>)))
            {
                services.AddScoped(service, type);
            }
        }
    }
}
