using Wst.Trios.Core.Shared.Kernel;

namespace Wst.Trios.Core.BrokerSide.Application.Order.Mappers;

public readonly struct SideMapper : IMap<Domain.Order.ESide, Integration.Dto.ESide>
{
    public Domain.Order.ESide Map(Integration.Dto.ESide right) =>
        right switch
        {
            Integration.Dto.ESide.Buy => Domain.Order.ESide.Buy,
            Integration.Dto.ESide.Sell => Domain.Order.ESide.Sell,
            _ => throw new ArgumentOutOfRangeException(nameof(right), right, null)
        };

    public Integration.Dto.ESide Map(Domain.Order.ESide left) =>
        left switch
        {
            Domain.Order.ESide.Buy => Integration.Dto.ESide.Buy,
            Domain.Order.ESide.Sell => Integration.Dto.ESide.Sell,
            _ => throw new ArgumentOutOfRangeException(nameof(left), left, null)
        };
}
