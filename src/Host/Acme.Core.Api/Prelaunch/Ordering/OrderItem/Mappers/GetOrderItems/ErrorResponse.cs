using Acme.Core.BuildingBlocks.Domain;
using GrpcOrderItem = Acme.Core.Protocol.Prelaunch.Ordering.OrderItem;

namespace Acme.Core.Api.Prelaunch.Ordering.OrderItem.Mappers.GetOrderItems;

// Domain error -> Protocol GetOrderItemsResponse (error envelope).
public readonly struct GetOrderItemsErrorResponse : IOnewayMap<LanguageExt.Common.Error, GrpcOrderItem.GetOrderItemsResponse>
{
    public GrpcOrderItem.GetOrderItemsResponse Map(LanguageExt.Common.Error left) => new()
    {
        Error = new GrpcOrderItem.GetOrderItemsError
        {
            Code = ErrorCodeClassifier.Classify(left),
            Message = left.Message
        }
    };
}
