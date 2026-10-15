using Template.Core.Ordering.Contracts.Dto;
using Template.Core.Shared.Kernel;
using GrpcOrder = Template.Core.Protocol.Prelaunch.Ordering.Order;

namespace Template.Core.Api.Prelaunch.Ordering.Order.Mappers;

// Integration DTO enum -> Protocol enum.
public readonly struct StatusMapper : IMap<EStatus, GrpcOrder.Status>
{
    public GrpcOrder.Status Map(EStatus left) => left switch
    {
        EStatus.New => GrpcOrder.Status.New,
        EStatus.InProgress => GrpcOrder.Status.InProgress,
        EStatus.Completed => GrpcOrder.Status.Completed,
        EStatus.Cancelled => GrpcOrder.Status.Cancelled,
        EStatus.Rejected => GrpcOrder.Status.Rejected,
        _ => throw new ArgumentOutOfRangeException(nameof(left), left, null)
    };

    public EStatus Map(GrpcOrder.Status left) => left switch
    {
        GrpcOrder.Status.New => EStatus.New,
        GrpcOrder.Status.InProgress => EStatus.InProgress,
        GrpcOrder.Status.Completed => EStatus.Completed,
        GrpcOrder.Status.Cancelled => EStatus.Cancelled,
        GrpcOrder.Status.Rejected => EStatus.Rejected,
        _ => throw new ArgumentOutOfRangeException(nameof(left), left, null)
    };
}
