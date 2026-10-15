using Acme.Core.BuildingBlocks.Domain;
using GrpcOrder = Acme.Core.Protocol.Prelaunch.Ordering.Order;

namespace Acme.Api.Infrastructure.CoreApi.Prelaunch.Ordering.Order.Mappers.DeleteOrder;

// Protocol DeleteOrderSuccess -> client response.
public readonly struct DeleteOrderResponse : IOnewayMap<GrpcOrder.DeleteOrderSuccess, Responses.DeleteOrderResponse>
{
    public Responses.DeleteOrderResponse Map(GrpcOrder.DeleteOrderSuccess left) => new();
}
