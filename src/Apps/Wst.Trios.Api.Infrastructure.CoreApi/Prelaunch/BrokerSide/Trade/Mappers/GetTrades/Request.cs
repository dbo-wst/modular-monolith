using Wst.Trios.Core.Shared.Kernel;
using GrpcTrade = Wst.Trios.Core.Protocol.Prelaunch.BrokerSide.Trade;

namespace Wst.Trios.Api.Infrastructure.CoreApi.Prelaunch.BrokerSide.Trade.Mappers.GetTrades;

// Client request -> Protocol GetTradesRequest.
public readonly struct GetTradesRequest : IOnewayMap<Requests.GetTradesRequest, GrpcTrade.GetTradesRequest>
{
    public GrpcTrade.GetTradesRequest Map(Requests.GetTradesRequest left) =>
        new() { PageNumber = left.PageNumber, PageSize = left.PageSize };
}
