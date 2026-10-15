namespace Acme.Core.BuildingBlocks.Domain;

/// <summary>
/// Something that happened in the domain. Every instance carries its own identity and timestamp, so two
/// events with the same payload are still distinct (record equality includes <see cref="EventId"/>).
/// </summary>
public abstract record DomainEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();

    public DateTimeOffset OccurredOn { get; init; } = DateTimeOffset.UtcNow;
}
