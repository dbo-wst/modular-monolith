using Wst.Trios.Core.Shared.Kernel;
using GrpcTrade = Wst.Trios.Core.Protocol.Prelaunch.BrokerSide.Trade;

namespace Wst.Trios.Api.Infrastructure.CoreApi.Prelaunch.BrokerSide.Trade.Mappers.CreateTrade;

// Protocol CreateTradeError -> ApiError.
public readonly struct CreateTradeErrorResponse : IOnewayMap<GrpcTrade.CreateTradeError?, ApiError>
{
    public ApiError Map(GrpcTrade.CreateTradeError? left) => ApiErrors.From(left?.Code, left?.Message);
}
