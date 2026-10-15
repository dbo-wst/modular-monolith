using Template.Core.BuildingBlocks.Domain;

namespace Template.Core.BuildingBlocks.Persistence;

/// <summary>
/// Scoped registry of the aggregates touched in the current unit of work. Repositories map aggregates to
/// persistence models, so EF Core's change tracker never sees the aggregates (or their events); repositories
/// therefore <see cref="Track"/> every aggregate they add, update or delete, and <see cref="UnitOfWork{TContext}"/>
/// drains the collected events after a successful save. Tracking is by reference, so events raised after
/// <see cref="Track"/> are still picked up.
/// </summary>
public sealed class DomainEventCollector
{
    private readonly List<IHasDomainEvents> _tracked = [];

    public void Track(IHasDomainEvents aggregate)
    {
        ArgumentNullException.ThrowIfNull(aggregate);
        if (!_tracked.Any(t => ReferenceEquals(t, aggregate)))
            _tracked.Add(aggregate);
    }

    /// <summary>
    /// Returns the pending events of all tracked aggregates (in tracking order), clears them on the aggregates
    /// and forgets the aggregates.
    /// </summary>
    public IReadOnlyList<DomainEvent> Drain()
    {
        var events = _tracked.SelectMany(a => a.DomainEvents).ToList();

        foreach (var aggregate in _tracked)
            aggregate.ClearDomainEvents();

        _tracked.Clear();
        return events;
    }
}
