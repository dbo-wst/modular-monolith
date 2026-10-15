using Acme.Core.BuildingBlocks.Domain;
using GrpcOrder = Acme.Core.Protocol.Prelaunch.Ordering.Order;

namespace Acme.Core.Api.Prelaunch.Ordering.Order.Mappers.GetOrders;

// Domain error -> Protocol GetOrdersResponse (error envelope).
public readonly struct GetOrdersErrorResponse : IOnewayMap<LanguageExt.Common.Error, GrpcOrder.GetOrdersResponse>
{
    public GrpcOrder.GetOrdersResponse Map(LanguageExt.Common.Error left) => new()
    {
        Error = new GrpcOrder.GetOrdersError
        {
            Code = ErrorCodeClassifier.Classify(left),
            Message = left.Message
        }
    };
}
