using Template.Core.BuildingBlocks.Domain;
using DomainEStatus = Template.Core.Ordering.Domain.Orders.OrderStatus;

namespace Template.Core.Ordering.Persistence.Orders.Mappers;

public readonly struct EStatusDboMapper : IMap<DomainEStatus, EStatusDbo>
{
    public DomainEStatus Map(EStatusDbo right) => right switch
    {
        EStatusDbo.New => DomainEStatus.New,
        EStatusDbo.InProgress => DomainEStatus.InProgress,
        EStatusDbo.Completed => DomainEStatus.Completed,
        EStatusDbo.Cancelled => DomainEStatus.Cancelled,
        EStatusDbo.Rejected => DomainEStatus.Rejected,
        _ => throw new ArgumentOutOfRangeException(nameof(right), right, null)
    };

    public EStatusDbo Map(DomainEStatus left) => left switch
    {
        DomainEStatus.New => EStatusDbo.New,
        DomainEStatus.InProgress => EStatusDbo.InProgress,
        DomainEStatus.Completed => EStatusDbo.Completed,
        DomainEStatus.Cancelled => EStatusDbo.Cancelled,
        DomainEStatus.Rejected => EStatusDbo.Rejected,
        _ => throw new ArgumentOutOfRangeException(nameof(left), left, null)
    };
}
