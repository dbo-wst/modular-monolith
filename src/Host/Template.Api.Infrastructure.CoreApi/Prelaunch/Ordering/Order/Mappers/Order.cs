using Template.Core.Protocol;
using Template.Core.Shared.Kernel;
using GrpcOrder = Template.Core.Protocol.Prelaunch.Ordering.Order;

namespace Template.Api.Infrastructure.CoreApi.Prelaunch.Ordering.Order.Mappers;

// Protocol Order -> client Model.
public readonly struct Order : IOnewayMap<GrpcOrder.Order, OrderDto>
{
    public OrderDto Map(GrpcOrder.Order left) => new(
        left.OrderId.AsGuid(),
        left.HasNumber ? left.Number : null,
        left.EffectiveDate?.AsDateOnly(),
        left.HasStatus ? ProtoMappers.Status.Map(left.Status) : null);
}
