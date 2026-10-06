using Wst.Trios.Core.BrokerSide.Integration.Dto;
using Wst.Trios.Core.Shared.Kernel;
using GrpcOrder = Wst.Trios.Core.Protocol.Prelaunch.BrokerSide.Order;

namespace Wst.Trios.Core.Api.Prelaunch.BrokerSide.Order.Mappers;

// Integration DTO enum -> Protocol enum.
public readonly struct SideMapper : IMap<ESide, GrpcOrder.Side>
{
    public ESide Map(GrpcOrder.Side right) => right switch
        {
            GrpcOrder.Side.Buy => ESide.Buy,
            GrpcOrder.Side.Sell => ESide.Sell,
            _ => throw new ArgumentOutOfRangeException(nameof(right), right, null)
        };

    public GrpcOrder.Side Map(ESide left) => left switch
    {
        ESide.Buy => GrpcOrder.Side.Buy,
        ESide.Sell => GrpcOrder.Side.Sell,
        _ => throw new ArgumentOutOfRangeException(nameof(left), left, null)
    };
}
