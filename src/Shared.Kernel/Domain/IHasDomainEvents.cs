namespace Wst.Trios.Core.Shared.Kernel.Domain;

public interface IHasDomainEvents
{
    public List<DomainEvent> DomainEvents { get; }
}
