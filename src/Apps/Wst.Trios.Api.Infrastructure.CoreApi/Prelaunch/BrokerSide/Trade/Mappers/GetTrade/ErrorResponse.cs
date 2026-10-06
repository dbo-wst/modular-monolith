using Wst.Trios.Core.Shared.Kernel;
using GrpcTrade = Wst.Trios.Core.Protocol.Prelaunch.BrokerSide.Trade;

namespace Wst.Trios.Api.Infrastructure.CoreApi.Prelaunch.BrokerSide.Trade.Mappers.GetTrade;

// Protocol GetTradeError -> ApiError.
public readonly struct GetTradeErrorResponse : IOnewayMap<GrpcTrade.GetTradeError?, ApiError>
{
    public ApiError Map(GrpcTrade.GetTradeError? left) => ApiErrors.From(left?.Code, left?.Message);
}
