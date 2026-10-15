using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Template.Core.BuildingBlocks.Domain;

namespace Template.Core.BuildingBlocks.Application.Communication;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the single <see cref="IMediator"/> shared by all modules, the default pipeline behaviours and the
    /// in-process <see cref="IDomainEventDispatcher"/>. Called once by the host; safe to call repeatedly.
    /// </summary>
    public static IServiceCollection AddMediator(this IServiceCollection services)
    {
        services.TryAddScoped<IMediator, Mediator>();
        services.TryAddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
        services.TryAddEnumerable(ServiceDescriptor.Scoped<IPipelineBehavior, ValidationBehavior>());
        return services;
    }

    /// <summary>
    /// Registers a behaviour that wraps commands only (never queries). Command behaviours run after every
    /// <see cref="IPipelineBehavior"/> (e.g. validation) and directly around the handler; among themselves
    /// the first registered runs outermost. Safe to call repeatedly with the same type.
    /// </summary>
    public static IServiceCollection AddCommandPipelineBehavior<TBehavior>(this IServiceCollection services)
        where TBehavior : class, ICommandPipelineBehavior
    {
        services.TryAddEnumerable(ServiceDescriptor.Scoped<ICommandPipelineBehavior, TBehavior>());
        return services;
    }

    /// <summary>
    /// Registers every command/query handler, domain event handler and FluentValidation validator of a module's
    /// Application assembly. Each request type may have exactly one handler across all modules; an event type may have many.
    /// </summary>
    public static IServiceCollection AddRequestHandlersFromAssembly(this IServiceCollection services, Assembly assembly)
    {
        var types = assembly.GetTypes().Where(t => t is { IsClass: true, IsAbstract: false }).ToList();

        foreach (var (service, implementation) in ClosedInterfaces(types, typeof(IRequestHandler<,>)))
        {
            var existing = services.FirstOrDefault(d => d.ServiceType == service);
            if (existing is not null && existing.ImplementationType != implementation)
            {
                throw new InvalidOperationException(
                    $"{service.GenericTypeArguments[0].Name} already has a handler ({existing.ImplementationType?.FullName}); " +
                    $"{implementation.FullName} cannot be registered as a second one.");
            }

            services.TryAddScoped(service, implementation);
        }

        foreach (var (service, implementation) in ClosedInterfaces(types, typeof(IDomainEventHandler<>)))
        {
            services.TryAddEnumerable(ServiceDescriptor.Scoped(service, implementation));
        }

        foreach (var (service, implementation) in ClosedInterfaces(types, typeof(IValidator<>)))
        {
            services.TryAddEnumerable(ServiceDescriptor.Scoped(service, implementation));
        }

        return services;
    }

    private static IEnumerable<(Type Service, Type Implementation)> ClosedInterfaces(
        IEnumerable<Type> types, Type openGeneric) =>
        types.SelectMany(t => t.GetInterfaces()
            .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == openGeneric)
            .Select(i => (i, t)));
}
