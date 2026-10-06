using Wst.Trios.Core.Shared.Kernel;
using GrpcOrder = Wst.Trios.Core.Protocol.Prelaunch.BrokerSide.Order;

namespace Wst.Trios.Api.Infrastructure.CoreApi.Prelaunch.BrokerSide.Order.Mappers;

// Client enum <-> Protocol enum.
public readonly struct SideMapper : IMap<ESide, GrpcOrder.Side>
{
    public ESide Map(GrpcOrder.Side right) => right switch
    {
        GrpcOrder.Side.Buy => ESide.Buy,
        GrpcOrder.Side.Sell => ESide.Sell,
        GrpcOrder.Side.Unspecified => ESide.Unspecified,
        _ => throw new ArgumentOutOfRangeException(nameof(right), right, null)
    };

    public GrpcOrder.Side Map(ESide left) => left switch
    {
        ESide.Buy => GrpcOrder.Side.Buy,
        ESide.Sell => GrpcOrder.Side.Sell,
        ESide.Unspecified => GrpcOrder.Side.Unspecified,
        _ => throw new ArgumentOutOfRangeException(nameof(left), left, null)
    };
}
