using Template.Core.Shared.Kernel;
using GrpcOrder = Template.Core.Protocol.Prelaunch.Ordering.Order;

namespace Template.Core.Api.Prelaunch.Ordering.Order.Mappers.DeleteOrder;

// Domain error -> Protocol DeleteOrderResponse (error envelope).
public readonly struct DeleteOrderErrorResponse : IOnewayMap<LanguageExt.Common.Error, GrpcOrder.DeleteOrderResponse>
{
    public GrpcOrder.DeleteOrderResponse Map(LanguageExt.Common.Error left) => new()
    {
        Error = new GrpcOrder.DeleteOrderError
        {
            Code = OrderErrorCode.Classify(left),
            Message = left.Message
        }
    };
}
