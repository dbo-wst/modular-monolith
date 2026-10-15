using Template.Core.BuildingBlocks.Domain;
using GrpcOrderItem = Template.Core.Protocol.Prelaunch.Ordering.OrderItem;

namespace Template.Core.Api.Prelaunch.Ordering.OrderItem.Mappers.GetOrderItem;

// Domain error -> Protocol GetOrderItemResponse (error envelope).
public readonly struct GetOrderItemErrorResponse : IOnewayMap<LanguageExt.Common.Error, GrpcOrderItem.GetOrderItemResponse>
{
    public GrpcOrderItem.GetOrderItemResponse Map(LanguageExt.Common.Error left) => new()
    {
        Error = new GrpcOrderItem.GetOrderItemError
        {
            Code = ErrorCodeClassifier.Classify(left),
            Message = left.Message
        }
    };
}
