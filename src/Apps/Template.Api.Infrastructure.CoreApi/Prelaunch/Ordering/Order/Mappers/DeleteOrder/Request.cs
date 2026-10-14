using Template.Core.Protocol;
using Template.Core.Shared.Kernel;
using GrpcOrder = Template.Core.Protocol.Prelaunch.Ordering.Order;

namespace Template.Api.Infrastructure.CoreApi.Prelaunch.Ordering.Order.Mappers.DeleteOrder;

// Client request -> Protocol DeleteOrderRequest.
public readonly struct DeleteOrderRequest : IOnewayMap<Requests.DeleteOrderRequest, GrpcOrder.DeleteOrderRequest>
{
    public GrpcOrder.DeleteOrderRequest Map(Requests.DeleteOrderRequest left) =>
        new() { OrderId = left.OrderId.AsByteString() };
}
