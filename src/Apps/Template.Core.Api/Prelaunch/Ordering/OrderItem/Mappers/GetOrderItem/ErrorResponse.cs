using Template.Core.Shared.Kernel;
using GrpcOrderItem = Template.Core.Protocol.Prelaunch.Ordering.OrderItem;

namespace Template.Core.Api.Prelaunch.Ordering.OrderItem.Mappers.GetOrderItem;

// Domain error -> Protocol GetOrderItemResponse (error envelope).
public readonly struct GetOrderItemErrorResponse : IOnewayMap<LanguageExt.Common.Error, GrpcOrderItem.GetOrderItemResponse>
{
    public GrpcOrderItem.GetOrderItemResponse Map(LanguageExt.Common.Error left) => new()
    {
        Error = new GrpcOrderItem.GetOrderItemError
        {
            Code = OrderItemErrorCode.Classify(left),
            Message = left.Message
        }
    };
}
