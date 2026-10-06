using System.Globalization;
using Wst.Trios.Core.Protocol;
using Wst.Trios.Core.Shared.Kernel;
using GrpcTrade = Wst.Trios.Core.Protocol.Prelaunch.BrokerSide.Trade;
using IntegrationRequests = Wst.Trios.Core.BrokerSide.Integration.Requests;

namespace Wst.Trios.Core.Api.Prelaunch.BrokerSide.Trade.Mappers.CreateTrade;

public readonly struct CreateTradeRequest : IOnewayMap<GrpcTrade.CreateTradeRequest, IntegrationRequests.CreateTradeRequest>
{
    public IntegrationRequests.CreateTradeRequest Map(GrpcTrade.CreateTradeRequest left) => new(
        OrderId: left.HasOrderId ? left.OrderId.AsGuid() : null,
        EffectiveDate: left.EffectiveDate?.AsDateOnly(),
        Quantity: left.HasQuantity ? decimal.Parse(left.Quantity, CultureInfo.InvariantCulture) : null,
        Price: left.HasPrice ? decimal.Parse(left.Price, CultureInfo.InvariantCulture) : null,
        Amount: left.HasAmount ? decimal.Parse(left.Amount, CultureInfo.InvariantCulture) : null);
}
