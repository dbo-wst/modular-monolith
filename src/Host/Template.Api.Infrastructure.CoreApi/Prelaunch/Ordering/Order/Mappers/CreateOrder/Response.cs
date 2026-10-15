using Template.Core.Shared.Kernel;
using GrpcOrder = Template.Core.Protocol.Prelaunch.Ordering.Order;

namespace Template.Api.Infrastructure.CoreApi.Prelaunch.Ordering.Order.Mappers.CreateOrder;

// Protocol CreateOrderSuccess -> client response.
public readonly struct CreateOrderResponse : IOnewayMap<GrpcOrder.CreateOrderSuccess, Responses.CreateOrderResponse>
{
    public Responses.CreateOrderResponse Map(GrpcOrder.CreateOrderSuccess left) =>
        new(ProtoMappers.Order.Map(left.Order));
}
