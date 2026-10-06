using Wst.Trios.Core.Shared.Kernel;
using DomainEStatus = Wst.Trios.Core.BrokerSide.Domain.Order.EStatus;

namespace Wst.Trios.Core.BrokerSide.Persistence.Order.Mappers;

public readonly struct EStatusDboMapper : IMap<DomainEStatus, EStatusDbo>
{
    public DomainEStatus Map(EStatusDbo right) => right switch
    {
        EStatusDbo.New => DomainEStatus.New,
        EStatusDbo.PartiallyFilled => DomainEStatus.PartiallyFilled,
        EStatusDbo.Filled => DomainEStatus.Filled,
        EStatusDbo.Cancelled => DomainEStatus.Cancelled,
        EStatusDbo.Rejected => DomainEStatus.Rejected,
        _ => throw new ArgumentOutOfRangeException(nameof(right), right, null)
    };

    public EStatusDbo Map(DomainEStatus left) => left switch
    {
        DomainEStatus.New => EStatusDbo.New,
        DomainEStatus.PartiallyFilled => EStatusDbo.PartiallyFilled,
        DomainEStatus.Filled => EStatusDbo.Filled,
        DomainEStatus.Cancelled => EStatusDbo.Cancelled,
        DomainEStatus.Rejected => EStatusDbo.Rejected,
        _ => throw new ArgumentOutOfRangeException(nameof(left), left, null)
    };
}
