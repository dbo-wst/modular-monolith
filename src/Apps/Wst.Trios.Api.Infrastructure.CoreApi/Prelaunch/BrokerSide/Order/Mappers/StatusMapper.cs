using Wst.Trios.Core.Shared.Kernel;
using GrpcOrder = Wst.Trios.Core.Protocol.Prelaunch.BrokerSide.Order;

namespace Wst.Trios.Api.Infrastructure.CoreApi.Prelaunch.BrokerSide.Order.Mappers;

// Client enum <-> Protocol enum.
public readonly struct StatusMapper : IMap<EStatus, GrpcOrder.Status>
{
    public EStatus Map(GrpcOrder.Status right) => right switch
    {
        GrpcOrder.Status.New => EStatus.New,
        GrpcOrder.Status.PartiallyFilled => EStatus.PartiallyFilled,
        GrpcOrder.Status.Filled => EStatus.Filled,
        GrpcOrder.Status.Cancelled => EStatus.Cancelled,
        GrpcOrder.Status.Rejected => EStatus.Rejected,
        GrpcOrder.Status.Unspecified => EStatus.Unspecified,
        _ => throw new ArgumentOutOfRangeException(nameof(right), right, null)
    };

    public GrpcOrder.Status Map(EStatus left) => left switch
    {
        EStatus.New => GrpcOrder.Status.New,
        EStatus.PartiallyFilled => GrpcOrder.Status.PartiallyFilled,
        EStatus.Filled => GrpcOrder.Status.Filled,
        EStatus.Cancelled => GrpcOrder.Status.Cancelled,
        EStatus.Rejected => GrpcOrder.Status.Rejected,
        _ => throw new ArgumentOutOfRangeException(nameof(left), left, null)
    };
}
