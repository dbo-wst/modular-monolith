using Template.Core.Shared.Kernel;
using GrpcOrderItem = Template.Core.Protocol.Prelaunch.Ordering.OrderItem;

namespace Template.Core.Api.Prelaunch.Ordering.OrderItem.Mappers.CreateOrderItem;

// Domain error -> Protocol CreateOrderItemResponse (error envelope).
public readonly struct CreateOrderItemErrorResponse : IOnewayMap<LanguageExt.Common.Error, GrpcOrderItem.CreateOrderItemResponse>
{
    public GrpcOrderItem.CreateOrderItemResponse Map(LanguageExt.Common.Error left) => new()
    {
        Error = new GrpcOrderItem.CreateOrderItemError
        {
            Code = OrderItemErrorCode.Classify(left),
            Message = left.Message
        }
    };
}
