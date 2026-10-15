using Template.Core.Shared.Kernel;
using GrpcOrder = Template.Core.Protocol.Prelaunch.Ordering.Order;

namespace Template.Core.Api.Prelaunch.Ordering.Order.Mappers.CreateOrder;

// Domain error -> Protocol CreateOrderResponse (error envelope).
public readonly struct CreateOrderErrorResponse : IOnewayMap<LanguageExt.Common.Error, GrpcOrder.CreateOrderResponse>
{
    public GrpcOrder.CreateOrderResponse Map(LanguageExt.Common.Error left) => new()
    {
        Error = new GrpcOrder.CreateOrderError
        {
            Code = OrderErrorCode.Classify(left),
            Message = left.Message
        }
    };
}
