using Wst.Trios.Core.Shared.Kernel;

namespace Wst.Trios.Core.BrokerSide.Application.Order.Mappers;

public readonly struct StatusMapper : IMap<Domain.Order.EStatus, Integration.Dto.EStatus>
{
    public Domain.Order.EStatus Map(Integration.Dto.EStatus right)
    {
        return right switch
        {
            Integration.Dto.EStatus.New => Domain.Order.EStatus.New,
            Integration.Dto.EStatus.PartiallyFilled => Domain.Order.EStatus.PartiallyFilled,
            Integration.Dto.EStatus.Filled => Domain.Order.EStatus.Filled,
            Integration.Dto.EStatus.Cancelled => Domain.Order.EStatus.Cancelled,
            Integration.Dto.EStatus.Rejected => Domain.Order.EStatus.Rejected,
            _ => throw new ArgumentOutOfRangeException(nameof(right), right, null)
        };
    }

    public Integration.Dto.EStatus Map(Domain.Order.EStatus left)
    {
        return left switch
        {
            Domain.Order.EStatus.New => Integration.Dto.EStatus.New,
            Domain.Order.EStatus.PartiallyFilled => Integration.Dto.EStatus.PartiallyFilled,
            Domain.Order.EStatus.Filled => Integration.Dto.EStatus.Filled,
            Domain.Order.EStatus.Cancelled => Integration.Dto.EStatus.Cancelled,
            Domain.Order.EStatus.Rejected => Integration.Dto.EStatus.Rejected,
            _ => throw new ArgumentOutOfRangeException(nameof(left), left, null)
        };
    }
}
