using Wst.Trios.Core.Protocol;
using Wst.Trios.Core.Shared.Kernel;
using GrpcTrade = Wst.Trios.Core.Protocol.Prelaunch.BrokerSide.Trade;
using IntegrationRequests = Wst.Trios.Core.BrokerSide.Integration.Requests;

namespace Wst.Trios.Core.Api.Prelaunch.BrokerSide.Trade.Mappers.DeleteTrade;

// Protocol request -> Integration request.
public readonly struct DeleteTradeRequest
    : IOnewayMap<GrpcTrade.DeleteTradeRequest, IntegrationRequests.DeleteTradeRequest>
{
    public IntegrationRequests.DeleteTradeRequest Map(GrpcTrade.DeleteTradeRequest left) =>
        new(left.TradeId.AsGuid());
}
