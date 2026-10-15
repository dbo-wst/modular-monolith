using Template.Core.Shared.Kernel;
using GrpcOrder = Template.Core.Protocol.Prelaunch.Ordering.Order;

namespace Template.Api.Infrastructure.CoreApi.Prelaunch.Ordering.Order.Mappers.GetOrder;

// Protocol GetOrderSuccess -> client response.
public readonly struct GetOrderResponse : IOnewayMap<GrpcOrder.GetOrderSuccess, Responses.GetOrderResponse>
{
    public Responses.GetOrderResponse Map(GrpcOrder.GetOrderSuccess left) =>
        new(ProtoMappers.Order.Map(left.Order));
}
