namespace Template.Core.Shared.Kernel.Domain;

public abstract class AggregateRoot<TId>: IHasDomainEvents
{
    public TId Id { get; protected set; } = default!;

    public List<DomainEvent> DomainEvents { get; } = [];

    protected void Raise(DomainEvent domainEvent)
    {
        DomainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        DomainEvents.Clear();
    }
}