using Wst.Trios.Core.Shared.Kernel;
using GrpcTrade = Wst.Trios.Core.Protocol.Prelaunch.BrokerSide.Trade;

namespace Wst.Trios.Api.Infrastructure.CoreApi.Prelaunch.BrokerSide.Trade.Mappers.DeleteTrade;

// Protocol DeleteTradeError -> ApiError.
public readonly struct DeleteTradeErrorResponse : IOnewayMap<GrpcTrade.DeleteTradeError?, ApiError>
{
    public ApiError Map(GrpcTrade.DeleteTradeError? left) => ApiErrors.From(left?.Code, left?.Message);
}
