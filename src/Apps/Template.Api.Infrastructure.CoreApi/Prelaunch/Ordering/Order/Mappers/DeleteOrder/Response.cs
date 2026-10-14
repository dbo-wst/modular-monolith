using Template.Core.Shared.Kernel;
using GrpcOrder = Template.Core.Protocol.Prelaunch.Ordering.Order;

namespace Template.Api.Infrastructure.CoreApi.Prelaunch.Ordering.Order.Mappers.DeleteOrder;

// Protocol DeleteOrderSuccess -> client response.
public readonly struct DeleteOrderResponse : IOnewayMap<GrpcOrder.DeleteOrderSuccess, Responses.DeleteOrderResponse>
{
    public Responses.DeleteOrderResponse Map(GrpcOrder.DeleteOrderSuccess left) => new();
}
