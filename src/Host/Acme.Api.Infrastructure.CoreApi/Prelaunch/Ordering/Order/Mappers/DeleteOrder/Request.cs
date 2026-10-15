using Acme.Core.Protocol;
using Acme.Core.BuildingBlocks.Domain;
using GrpcOrder = Acme.Core.Protocol.Prelaunch.Ordering.Order;

namespace Acme.Api.Infrastructure.CoreApi.Prelaunch.Ordering.Order.Mappers.DeleteOrder;

// Client request -> Protocol DeleteOrderRequest.
public readonly struct DeleteOrderRequest : IOnewayMap<Requests.DeleteOrderRequest, GrpcOrder.DeleteOrderRequest>
{
    public GrpcOrder.DeleteOrderRequest Map(Requests.DeleteOrderRequest left) =>
        new() { OrderId = left.OrderId.AsByteString() };
}
