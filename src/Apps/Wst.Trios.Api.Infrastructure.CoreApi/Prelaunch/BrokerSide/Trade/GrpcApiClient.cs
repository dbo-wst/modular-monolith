using LanguageExt;
using Wst.Trios.Api.Infrastructure.CoreApi.Prelaunch.BrokerSide.Trade.Requests;
using Wst.Trios.Api.Infrastructure.CoreApi.Prelaunch.BrokerSide.Trade.Responses;
using GrpcTrade = Wst.Trios.Core.Protocol.Prelaunch.BrokerSide.Trade;

namespace Wst.Trios.Api.Infrastructure.CoreApi.Prelaunch.BrokerSide.Trade;

public sealed class GrpcApiClient(GrpcTrade.TradeService.TradeServiceClient client) : IGrpcApiClient
{
    public async Task<Fin<CreateTradeResponse>> CreateAsync(CreateTradeRequest request, CancellationToken cancellationToken = default)
    {
        var response = await client.CreateTradeAsync(
            ProtoMappers.CreateTradeRequest.Map(request), cancellationToken: cancellationToken);

        return response.ResultCase == GrpcTrade.CreateTradeResponse.ResultOneofCase.Success
            ? Fin<CreateTradeResponse>.Succ(ProtoMappers.CreateTradeResponse.Map(response.Success))
            : Fin<CreateTradeResponse>.Fail(ProtoMappers.CreateTradeErrorResponse.Map(response.Error));
    }

    public async Task<Fin<GetTradeResponse>> GetAsync(GetTradeRequest request, CancellationToken cancellationToken = default)
    {
        var response = await client.GetTradeAsync(
            ProtoMappers.GetTradeRequest.Map(request), cancellationToken: cancellationToken);

        return response.ResultCase == GrpcTrade.GetTradeResponse.ResultOneofCase.Success
            ? Fin<GetTradeResponse>.Succ(ProtoMappers.GetTradeResponse.Map(response.Success))
            : Fin<GetTradeResponse>.Fail(ProtoMappers.GetTradeErrorResponse.Map(response.Error));
    }

    public async Task<Fin<GetTradesResponse>> ListAsync(GetTradesRequest request, CancellationToken cancellationToken = default)
    {
        var response = await client.GetTradesAsync(
            ProtoMappers.GetTradesRequest.Map(request), cancellationToken: cancellationToken);

        return response.ResultCase == GrpcTrade.GetTradesResponse.ResultOneofCase.Success
            ? Fin<GetTradesResponse>.Succ(ProtoMappers.GetTradesResponse.Map(response.Success))
            : Fin<GetTradesResponse>.Fail(ProtoMappers.GetTradesErrorResponse.Map(response.Error));
    }

    public async Task<Fin<UpdateTradeResponse>> UpdateAsync(UpdateTradeRequest request, CancellationToken cancellationToken = default)
    {
        var response = await client.UpdateTradeAsync(
            ProtoMappers.UpdateTradeRequest.Map(request), cancellationToken: cancellationToken);

        return response.ResultCase == GrpcTrade.UpdateTradeResponse.ResultOneofCase.Success
            ? Fin<UpdateTradeResponse>.Succ(ProtoMappers.UpdateTradeResponse.Map(response.Success))
            : Fin<UpdateTradeResponse>.Fail(ProtoMappers.UpdateTradeErrorResponse.Map(response.Error));
    }

    public async Task<Fin<DeleteTradeResponse>> DeleteAsync(DeleteTradeRequest request, CancellationToken cancellationToken = default)
    {
        var response = await client.DeleteTradeAsync(
            ProtoMappers.DeleteTradeRequest.Map(request), cancellationToken: cancellationToken);

        return response.ResultCase == GrpcTrade.DeleteTradeResponse.ResultOneofCase.Deleted
            ? Fin<DeleteTradeResponse>.Succ(ProtoMappers.DeleteTradeResponse.Map(response.Deleted))
            : Fin<DeleteTradeResponse>.Fail(ProtoMappers.DeleteTradeErrorResponse.Map(response.Error));
    }
}
