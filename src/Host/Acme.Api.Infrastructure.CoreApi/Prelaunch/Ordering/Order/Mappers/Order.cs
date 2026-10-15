using Acme.Core.Protocol;
using Acme.Core.BuildingBlocks.Domain;
using GrpcOrder = Acme.Core.Protocol.Prelaunch.Ordering.Order;

namespace Acme.Api.Infrastructure.CoreApi.Prelaunch.Ordering.Order.Mappers;

// Protocol Order -> client Model.
public readonly struct Order : IOnewayMap<GrpcOrder.Order, OrderDto>
{
    public OrderDto Map(GrpcOrder.Order left) => new(
        left.OrderId.AsGuid(),
        left.HasNumber ? left.Number : null,
        left.EffectiveDate?.AsDateOnly(),
        left.HasStatus ? ProtoMappers.Status.Map(left.Status) : null);
}
