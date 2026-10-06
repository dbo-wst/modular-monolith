using System.Globalization;
using Wst.Trios.Core.Protocol;
using Wst.Trios.Core.Shared.Kernel;
using GrpcTrade = Wst.Trios.Core.Protocol.Prelaunch.BrokerSide.Trade;
using IntegrationRequests = Wst.Trios.Core.BrokerSide.Integration.Requests;

namespace Wst.Trios.Core.Api.Prelaunch.BrokerSide.Trade.Mappers.UpdateTrade;

public readonly struct UpdateTradeRequest : IOnewayMap<GrpcTrade.UpdateTradeRequest, IntegrationRequests.UpdateTradeRequest>
{
    public IntegrationRequests.UpdateTradeRequest Map(GrpcTrade.UpdateTradeRequest left) => new(
        TradeId: left.TradeId.AsGuid(),
        OrderId: left.HasOrderId ? left.OrderId.AsGuid() : null,
        EffectiveDate: left.EffectiveDate?.AsDateOnly(),
        Quantity: left.HasQuantity ? decimal.Parse(left.Quantity, CultureInfo.InvariantCulture) : null,
        Price: left.HasPrice ? decimal.Parse(left.Price, CultureInfo.InvariantCulture) : null,
        Amount: left.HasAmount ? decimal.Parse(left.Amount, CultureInfo.InvariantCulture) : null);
}
