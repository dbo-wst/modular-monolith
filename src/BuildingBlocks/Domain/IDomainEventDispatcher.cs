using LanguageExt;

namespace Acme.Core.BuildingBlocks.Domain;

/// <summary>
/// Port for publishing domain events in-process to their handlers. Declared here so persistence adapters
/// depend only on the domain building block; the implementation lives in BuildingBlocks.Application.
/// </summary>
public interface IDomainEventDispatcher
{
    /// <summary>Dispatches the events in order; stops at and returns the first failure.</summary>
    Task<Fin<Unit>> DispatchAsync(IEnumerable<DomainEvent> domainEvents, CancellationToken cancellationToken = default);
}
