using System.Globalization;
using Wst.Trios.Core.Protocol;
using Wst.Trios.Core.Shared.Kernel;
using GrpcTrade = Wst.Trios.Core.Protocol.Prelaunch.BrokerSide.Trade;

namespace Wst.Trios.Core.Api.Prelaunch.BrokerSide.Trade.Mappers;

public readonly struct Trade : IOnewayMap<Core.BrokerSide.Integration.Dto.Trade, GrpcTrade.Trade>
{
    public GrpcTrade.Trade Map(Core.BrokerSide.Integration.Dto.Trade left)
    {
        var proto = new GrpcTrade.Trade
        {
            TradeId = left.TradeId.AsByteString()
        };

        if (left.OrderId is { } orderId) proto.OrderId = orderId.AsByteString();
        if (left.EffectiveDate is { } effectiveDate) proto.EffectiveDate = effectiveDate.AsTimestamp();
        if (left.Quantity is { } Quantity) proto.Quantity = Quantity.ToString(CultureInfo.InvariantCulture);
        if (left.Price is { } price) proto.Price = price.ToString(CultureInfo.InvariantCulture);
        if (left.Amount is { } amount) proto.Amount = amount.ToString(CultureInfo.InvariantCulture);

        return proto;
    }
}

