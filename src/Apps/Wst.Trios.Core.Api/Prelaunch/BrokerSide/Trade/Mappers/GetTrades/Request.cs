using Wst.Trios.Core.Shared.Kernel;
using GrpcTrade = Wst.Trios.Core.Protocol.Prelaunch.BrokerSide.Trade;
using IntegrationRequests = Wst.Trios.Core.BrokerSide.Integration.Requests;

namespace Wst.Trios.Core.Api.Prelaunch.BrokerSide.Trade.Mappers.GetTrades;

// Protocol request -> Integration request.
public readonly struct GetTradesRequest
    : IOnewayMap<GrpcTrade.GetTradesRequest, IntegrationRequests.GetListTradeRequest>
{
    public IntegrationRequests.GetListTradeRequest Map(GrpcTrade.GetTradesRequest left) =>
        new(left.PageNumber, left.PageSize);
}
