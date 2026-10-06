using Grpc.Core;
using Wst.Trios.Core.Shared.Kernel.Communication;
using GrpcProtocol = Wst.Trios.Core.Protocol.Prelaunch.BrokerSide.Trade;

namespace Wst.Trios.Core.Api.Prelaunch.BrokerSide.Trade;

public sealed class GrpcService(IMediator mediator) : GrpcProtocol.TradeService.TradeServiceBase
{
    public override async Task<GrpcProtocol.CreateTradeResponse> CreateTrade(GrpcProtocol.CreateTradeRequest request, ServerCallContext context)
    {
        var result = await mediator.SendAsync(ProtoMappers.CreateTradeRequest.Map(request), context.CancellationToken);

        return result.Match(
            Succ: response => ProtoMappers.CreateTradeResponse.Map(response.Trade),
            Fail: ProtoMappers.CreateTradeErrorResponse.Map);
    }

    public override async Task<GrpcProtocol.GetTradeResponse> GetTrade(GrpcProtocol.GetTradeRequest request, ServerCallContext context)
    {
        var result = await mediator.SendAsync(ProtoMappers.GetTradeRequest.Map(request), context.CancellationToken);

        return result.Match(
            Succ: response => ProtoMappers.GetTradeResponse.Map(response.Trade),
            Fail: ProtoMappers.GetTradeErrorResponse.Map);
    }

    public override async Task<GrpcProtocol.GetTradesResponse> GetTrades(GrpcProtocol.GetTradesRequest request, ServerCallContext context)
    {
        var result = await mediator.SendAsync(ProtoMappers.GetTradesRequest.Map(request), context.CancellationToken);

        return result.Match(
            Succ: response => ProtoMappers.GetTradesResponse.Map((response.Trades, request.PageNumber, request.PageSize)),
            Fail: ProtoMappers.GetTradesErrorResponse.Map);
    }

    public override async Task<GrpcProtocol.UpdateTradeResponse> UpdateTrade(GrpcProtocol.UpdateTradeRequest request, ServerCallContext context)
    {
        var result = await mediator.SendAsync(ProtoMappers.UpdateTradeRequest.Map(request), context.CancellationToken);

        return result.Match(
            Succ: response => ProtoMappers.UpdateTradeResponse.Map(response.Trade),
            Fail: ProtoMappers.UpdateTradeErrorResponse.Map);
    }

    public override async Task<GrpcProtocol.DeleteTradeResponse> DeleteTrade(GrpcProtocol.DeleteTradeRequest request, ServerCallContext context)
    {
        var result = await mediator.SendAsync(ProtoMappers.DeleteTradeRequest.Map(request), context.CancellationToken);

        return result.Match(
            Succ: _ => ProtoMappers.DeleteTradeResponse.Map(request.TradeId),
            Fail: ProtoMappers.DeleteTradeErrorResponse.Map);
    }
}
