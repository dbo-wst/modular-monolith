using Wst.Trios.Core.Shared.Kernel;

namespace Wst.Trios.Core.BrokerSide.Persistence.Order.Mappers;

public readonly struct ESideDboMapper : IMap<Domain.Order.ESide, ESideDbo>
{
    public Domain.Order.ESide Map(ESideDbo right)
    {
        return right switch
        {
            ESideDbo.Buy => Domain.Order.ESide.Buy,
            ESideDbo.Sell => Domain.Order.ESide.Sell,
            _ => throw new ArgumentOutOfRangeException(nameof(right), right, null)
        };
    }

    public ESideDbo Map(Domain.Order.ESide left)
    {
        return left switch
        {
            Domain.Order.ESide.Buy => ESideDbo.Buy,
            Domain.Order.ESide.Sell => ESideDbo.Sell,
            _ => throw new ArgumentOutOfRangeException(nameof(left), left, null)
        };
    }
}
