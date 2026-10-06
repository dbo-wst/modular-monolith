using System.Globalization;
using Wst.Trios.Core.Protocol;
using Wst.Trios.Core.Shared.Kernel;
using GrpcTrade = Wst.Trios.Core.Protocol.Prelaunch.BrokerSide.Trade;

namespace Wst.Trios.Api.Infrastructure.CoreApi.Prelaunch.BrokerSide.Trade.Mappers;

// Protocol Trade -> client Model.
public readonly struct Trade : IOnewayMap<GrpcTrade.Trade, TradeDto>
{
    public TradeDto Map(GrpcTrade.Trade left) => new(
        left.TradeId.AsGuid(),
        left.HasOrderId ? left.OrderId.AsGuid() : null,
        left.EffectiveDate?.AsDateOnly(),
        left.HasQuantity ? decimal.Parse(left.Quantity, CultureInfo.InvariantCulture) : null,
        left.HasPrice ? decimal.Parse(left.Price, CultureInfo.InvariantCulture) : null,
        left.HasAmount ? decimal.Parse(left.Amount, CultureInfo.InvariantCulture) : null);
}
