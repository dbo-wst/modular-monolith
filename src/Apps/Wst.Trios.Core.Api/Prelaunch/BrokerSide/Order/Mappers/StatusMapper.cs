using Wst.Trios.Core.BrokerSide.Integration.Dto;
using Wst.Trios.Core.Shared.Kernel;
using GrpcOrder = Wst.Trios.Core.Protocol.Prelaunch.BrokerSide.Order;

namespace Wst.Trios.Core.Api.Prelaunch.BrokerSide.Order.Mappers;

// Integration DTO enum -> Protocol enum.
public readonly struct StatusMapper : IMap<EStatus, GrpcOrder.Status>
{
    public GrpcOrder.Status Map(EStatus left) => left switch
    {
        EStatus.New => GrpcOrder.Status.New,
        EStatus.PartiallyFilled => GrpcOrder.Status.PartiallyFilled,
        EStatus.Filled => GrpcOrder.Status.Filled,
        EStatus.Cancelled => GrpcOrder.Status.Cancelled,
        EStatus.Rejected => GrpcOrder.Status.Rejected,
        _ => throw new ArgumentOutOfRangeException(nameof(left), left, null)
    };

    public EStatus Map(GrpcOrder.Status left) => left switch
    {
        GrpcOrder.Status.New => EStatus.New,
        GrpcOrder.Status.PartiallyFilled => EStatus.PartiallyFilled,
        GrpcOrder.Status.Filled => EStatus.Filled,
        GrpcOrder.Status.Cancelled => EStatus.Cancelled,
        GrpcOrder.Status.Rejected => EStatus.Rejected,
        _ => throw new ArgumentOutOfRangeException(nameof(left), left, null)
    };
}
