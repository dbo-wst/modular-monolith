using Wst.Trios.Core.Protocol;
using Wst.Trios.Core.Shared.Kernel;
using GrpcTrade = Wst.Trios.Core.Protocol.Prelaunch.BrokerSide.Trade;
using IntegrationRequests = Wst.Trios.Core.BrokerSide.Integration.Requests;

namespace Wst.Trios.Core.Api.Prelaunch.BrokerSide.Trade.Mappers.GetTrade;

// Protocol request -> Integration request.
public readonly struct GetTradeRequest
    : IOnewayMap<GrpcTrade.GetTradeRequest, IntegrationRequests.GetTradeRequest>
{
    public IntegrationRequests.GetTradeRequest Map(GrpcTrade.GetTradeRequest left) =>
        new(left.TradeId.AsGuid());
}
