using Template.Core.BuildingBlocks.Domain;
using GrpcOrderItem = Template.Core.Protocol.Prelaunch.Ordering.OrderItem;

namespace Template.Core.Api.Prelaunch.Ordering.OrderItem.Mappers.CreateOrderItem;

// Domain error -> Protocol CreateOrderItemResponse (error envelope).
public readonly struct CreateOrderItemErrorResponse : IOnewayMap<LanguageExt.Common.Error, GrpcOrderItem.CreateOrderItemResponse>
{
    public GrpcOrderItem.CreateOrderItemResponse Map(LanguageExt.Common.Error left) => new()
    {
        Error = new GrpcOrderItem.CreateOrderItemError
        {
            Code = ErrorCodeClassifier.Classify(left),
            Message = left.Message
        }
    };
}
