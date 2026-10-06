using Wst.Trios.Core.Shared.Kernel;
using GrpcTrade = Wst.Trios.Core.Protocol.Prelaunch.BrokerSide.Trade;

namespace Wst.Trios.Api.Infrastructure.CoreApi.Prelaunch.BrokerSide.Trade.Mappers.GetTrades;

// Protocol GetTradesSuccess -> client response.
public readonly struct GetTradesResponse : IOnewayMap<GrpcTrade.GetTradesSuccess, Responses.GetTradesResponse>
{
    public Responses.GetTradesResponse Map(GrpcTrade.GetTradesSuccess left)
    {
        IReadOnlyList<TradeDto> items = left.Items.Select(ProtoMappers.Trade.Map).ToList();
        return new Responses.GetTradesResponse(items);
    }
}
