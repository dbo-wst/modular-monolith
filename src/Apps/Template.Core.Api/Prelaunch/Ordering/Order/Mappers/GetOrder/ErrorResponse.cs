using Template.Core.Shared.Kernel;
using GrpcOrder = Template.Core.Protocol.Prelaunch.Ordering.Order;

namespace Template.Core.Api.Prelaunch.Ordering.Order.Mappers.GetOrder;

// Domain error -> Protocol GetOrderResponse (error envelope).
public readonly struct GetOrderErrorResponse : IOnewayMap<LanguageExt.Common.Error, GrpcOrder.GetOrderResponse>
{
    public GrpcOrder.GetOrderResponse Map(LanguageExt.Common.Error left) => new()
    {
        Error = new GrpcOrder.GetOrderError
        {
            Code = OrderErrorCode.Classify(left),
            Message = left.Message
        }
    };
}
