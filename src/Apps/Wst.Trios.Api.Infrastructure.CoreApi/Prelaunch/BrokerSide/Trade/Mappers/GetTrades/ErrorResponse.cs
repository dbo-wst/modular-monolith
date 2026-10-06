using Wst.Trios.Core.Shared.Kernel;
using GrpcTrade = Wst.Trios.Core.Protocol.Prelaunch.BrokerSide.Trade;

namespace Wst.Trios.Api.Infrastructure.CoreApi.Prelaunch.BrokerSide.Trade.Mappers.GetTrades;

// Protocol GetTradesError -> ApiError.
public readonly struct GetTradesErrorResponse : IOnewayMap<GrpcTrade.GetTradesError?, ApiError>
{
    public ApiError Map(GrpcTrade.GetTradesError? left) => ApiErrors.From(left?.Code, left?.Message);
}
