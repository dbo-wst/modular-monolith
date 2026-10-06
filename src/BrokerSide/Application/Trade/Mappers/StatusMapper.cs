using Wst.Trios.Core.Shared.Kernel;

namespace Wst.Trios.Core.BrokerSide.Application.Trade.Mappers;

public readonly struct StatusMapper : IMap<Domain.Trade.EStatus, Integration.Dto.EStatus>
{
    public Domain.Trade.EStatus Map(Integration.Dto.EStatus right)
    {
        return right switch
        {
            Integration.Dto.EStatus.New => Domain.Trade.EStatus.New,
            Integration.Dto.EStatus.PartiallyFilled => Domain.Trade.EStatus.PartiallyFilled,
            Integration.Dto.EStatus.Filled => Domain.Trade.EStatus.Filled,
            Integration.Dto.EStatus.Cancelled => Domain.Trade.EStatus.Cancelled,
            Integration.Dto.EStatus.Rejected => Domain.Trade.EStatus.Rejected,
            _ => throw new ArgumentOutOfRangeException(nameof(right), right, null)
        };
    }

    public Integration.Dto.EStatus Map(Domain.Trade.EStatus left)
    {
        return left switch
        {
            Domain.Trade.EStatus.New => Integration.Dto.EStatus.New,
            Domain.Trade.EStatus.PartiallyFilled => Integration.Dto.EStatus.PartiallyFilled,
            Domain.Trade.EStatus.Filled => Integration.Dto.EStatus.Filled,
            Domain.Trade.EStatus.Cancelled => Integration.Dto.EStatus.Cancelled,
            Domain.Trade.EStatus.Rejected => Integration.Dto.EStatus.Rejected,
            _ => throw new ArgumentOutOfRangeException(nameof(left), left, null)
        };
    }
}
