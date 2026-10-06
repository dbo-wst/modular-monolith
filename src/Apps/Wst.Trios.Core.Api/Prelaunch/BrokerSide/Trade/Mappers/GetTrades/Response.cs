using Wst.Trios.Core.Shared.Kernel;
using GrpcTrade = Wst.Trios.Core.Protocol.Prelaunch.BrokerSide.Trade;

namespace Wst.Trios.Core.Api.Prelaunch.BrokerSide.Trade.Mappers.GetTrades;

// Paged Integration DTOs -> Protocol GetTradesResponse (success envelope).
public readonly struct GetTradesResponse
    : IOnewayMap<(IReadOnlyList<Core.BrokerSide.Integration.Dto.Trade> Trades, int PageNumber, int PageSize), GrpcTrade.GetTradesResponse>
{
    public GrpcTrade.GetTradesResponse Map((IReadOnlyList<Core.BrokerSide.Integration.Dto.Trade> Trades, int PageNumber, int PageSize) left)
    {
        var success = new GrpcTrade.GetTradesSuccess
        {
            PageNumber = left.PageNumber,
            PageSize = left.PageSize
        };
        success.Items.AddRange(left.Trades.Select(ProtoMappers.Trade.Map));

        return new GrpcTrade.GetTradesResponse { Success = success };
    }
}
