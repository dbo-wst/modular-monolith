using Wst.Trios.Core.Shared.Kernel;
using GrpcTrade = Wst.Trios.Core.Protocol.Prelaunch.BrokerSide.Trade;

namespace Wst.Trios.Api.Infrastructure.CoreApi.Prelaunch.BrokerSide.Trade.Mappers.UpdateTrade;

// Protocol UpdateTradeError -> ApiError.
public readonly struct UpdateTradeErrorResponse : IOnewayMap<GrpcTrade.UpdateTradeError?, ApiError>
{
    public ApiError Map(GrpcTrade.UpdateTradeError? left) => ApiErrors.From(left?.Code, left?.Message);
}
