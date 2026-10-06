using System.Globalization;
using Wst.Trios.Core.Protocol;
using Wst.Trios.Core.Shared.Kernel;
using GrpcTrade = Wst.Trios.Core.Protocol.Prelaunch.BrokerSide.Trade;

namespace Wst.Trios.Api.Infrastructure.CoreApi.Prelaunch.BrokerSide.Trade.Mappers.UpdateTrade;

// Client request -> Protocol UpdateTradeRequest.
public readonly struct UpdateTradeRequest : IOnewayMap<Requests.UpdateTradeRequest, GrpcTrade.UpdateTradeRequest>
{
    public GrpcTrade.UpdateTradeRequest Map(Requests.UpdateTradeRequest left)
    {
        var proto = new GrpcTrade.UpdateTradeRequest
        {
            TradeId = left.TradeId.AsByteString()
        };

        if (left.OrderId is { } orderId) proto.OrderId = orderId.AsByteString();
        if (left.EffectiveDate is { } effectiveDate) proto.EffectiveDate = effectiveDate.AsTimestamp();
        if (left.Quantity is { } quantity) proto.Quantity = quantity.ToString(CultureInfo.InvariantCulture);
        if (left.Price is { } price) proto.Price = price.ToString(CultureInfo.InvariantCulture);
        if (left.Amount is { } amount) proto.Amount = amount.ToString(CultureInfo.InvariantCulture);

        return proto;
    }
}
