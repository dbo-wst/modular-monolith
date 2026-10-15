using System.Collections.Concurrent;
using LanguageExt;
using Microsoft.Extensions.DependencyInjection;
using Template.Core.BuildingBlocks.Domain;

namespace Template.Core.BuildingBlocks.Application.Communication;

/// <summary>
/// Dispatches each event to every registered <see cref="IDomainEventHandler{TEvent}"/> of its runtime type,
/// in event order and handler registration order, resolved from the current scope. Stops at the first failure.
/// </summary>
public sealed class DomainEventDispatcher(IServiceProvider provider) : IDomainEventDispatcher
{
    public async Task<Fin<Unit>> DispatchAsync(
        IEnumerable<DomainEvent> domainEvents,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(domainEvents);

        foreach (var domainEvent in domainEvents)
        {
            var result = await DomainEventHandlerWrapper.For(domainEvent.GetType())
                .HandleAsync(domainEvent, provider, cancellationToken);

            if (result.IsFail)
                return result;
        }

        return Fin<Unit>.Succ(Unit.Default);
    }
}

/// <summary>Typed bridge from a runtime event type to its handlers; built once per event type and cached.</summary>
internal abstract class DomainEventHandlerWrapper
{
    private static readonly ConcurrentDictionary<Type, DomainEventHandlerWrapper> Cache = new();

    public static DomainEventHandlerWrapper For(Type eventType) =>
        Cache.GetOrAdd(eventType, static type => (DomainEventHandlerWrapper)Activator.CreateInstance(
            typeof(DomainEventHandlerWrapperImpl<>).MakeGenericType(type))!);

    public abstract Task<Fin<Unit>> HandleAsync(DomainEvent domainEvent, IServiceProvider sp, CancellationToken ct);
}

internal sealed class DomainEventHandlerWrapperImpl<TEvent> : DomainEventHandlerWrapper
    where TEvent : DomainEvent
{
    public override async Task<Fin<Unit>> HandleAsync(DomainEvent domainEvent, IServiceProvider sp, CancellationToken ct)
    {
        var typedEvent = (TEvent)domainEvent;

        foreach (var handler in sp.GetServices<IDomainEventHandler<TEvent>>())
        {
            var result = await handler.HandleAsync(typedEvent, ct);
            if (result.IsFail)
                return result;
        }

        return Fin<Unit>.Succ(Unit.Default);
    }
}
