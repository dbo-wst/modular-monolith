using Template.Core.Shared.Kernel;
using GrpcOrderItem = Template.Core.Protocol.Prelaunch.Ordering.OrderItem;

namespace Template.Core.Api.Prelaunch.Ordering.OrderItem.Mappers.DeleteOrderItem;

// Domain error -> Protocol DeleteOrderItemResponse (error envelope).
public readonly struct DeleteOrderItemErrorResponse : IOnewayMap<LanguageExt.Common.Error, GrpcOrderItem.DeleteOrderItemResponse>
{
    public GrpcOrderItem.DeleteOrderItemResponse Map(LanguageExt.Common.Error left) => new()
    {
        Error = new GrpcOrderItem.DeleteOrderItemError
        {
            Code = OrderItemErrorCode.Classify(left),
            Message = left.Message
        }
    };
}
