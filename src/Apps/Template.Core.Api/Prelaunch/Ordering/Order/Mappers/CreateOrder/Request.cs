using Template.Core.Protocol;
using Template.Core.Shared.Kernel;
using GrpcOrder = Template.Core.Protocol.Prelaunch.Ordering.Order;
using IntegrationRequests = Template.Core.Ordering.Integration.Requests;

namespace Template.Core.Api.Prelaunch.Ordering.Order.Mappers.CreateOrder;

public readonly struct CreateOrderRequest : IOnewayMap<GrpcOrder.CreateOrderRequest, IntegrationRequests.CreateOrderRequest>
{
    public IntegrationRequests.CreateOrderRequest Map(GrpcOrder.CreateOrderRequest left) => new(
        Number: left.HasNumber ? left.Number : null,
        EffectiveDate: left.EffectiveDate?.AsDateOnly());
}
