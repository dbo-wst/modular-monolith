using System.Globalization;
using Wst.Trios.Core.Protocol;
using Wst.Trios.Core.Shared.Kernel;
using GrpcTrade = Wst.Trios.Core.Protocol.Prelaunch.BrokerSide.Trade;

namespace Wst.Trios.Api.Infrastructure.CoreApi.Prelaunch.BrokerSide.Trade.Mappers.CreateTrade;

// Client request -> Protocol CreateTradeRequest.
public readonly struct CreateTradeRequest : IOnewayMap<Requests.CreateTradeRequest, GrpcTrade.CreateTradeRequest>
{
    public GrpcTrade.CreateTradeRequest Map(Requests.CreateTradeRequest left)
    {
        var proto = new GrpcTrade.CreateTradeRequest();

        if (left.OrderId is { } orderId) proto.OrderId = orderId.AsByteString();
        if (left.EffectiveDate is { } effectiveDate) proto.EffectiveDate = effectiveDate.AsTimestamp();
        if (left.Quantity is { } quantity) proto.Quantity = quantity.ToString(CultureInfo.InvariantCulture);
        if (left.Price is { } price) proto.Price = price.ToString(CultureInfo.InvariantCulture);
        if (left.Amount is { } amount) proto.Amount = amount.ToString(CultureInfo.InvariantCulture);

        return proto;
    }
}
