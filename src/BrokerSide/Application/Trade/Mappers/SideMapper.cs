using Wst.Trios.Core.Shared.Kernel;

namespace Wst.Trios.Core.BrokerSide.Application.Trade.Mappers;

public readonly struct SideMapper : IMap<Domain.Trade.ESide, Integration.Dto.ESide>
{
    public Domain.Trade.ESide Map(Integration.Dto.ESide right) =>
        right switch
        {
            Integration.Dto.ESide.Buy => Domain.Trade.ESide.Buy,
            Integration.Dto.ESide.Sell => Domain.Trade.ESide.Sell,
            _ => throw new ArgumentOutOfRangeException(nameof(right), right, null)
        };

    public Integration.Dto.ESide Map(Domain.Trade.ESide left) =>
        left switch
        {
            Domain.Trade.ESide.Buy => Integration.Dto.ESide.Buy,
            Domain.Trade.ESide.Sell => Integration.Dto.ESide.Sell,
            _ => throw new ArgumentOutOfRangeException(nameof(left), left, null)
        };
}
