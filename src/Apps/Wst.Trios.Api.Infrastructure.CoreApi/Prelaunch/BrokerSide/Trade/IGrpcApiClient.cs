using LanguageExt;
using Wst.Trios.Api.Infrastructure.CoreApi.Prelaunch.BrokerSide.Trade.Requests;
using Wst.Trios.Api.Infrastructure.CoreApi.Prelaunch.BrokerSide.Trade.Responses;

namespace Wst.Trios.Api.Infrastructure.CoreApi.Prelaunch.BrokerSide.Trade;

public interface IGrpcApiClient
{
    Task<Fin<CreateTradeResponse>> CreateAsync(CreateTradeRequest request, CancellationToken cancellationToken = default);

    Task<Fin<GetTradeResponse>> GetAsync(GetTradeRequest request, CancellationToken cancellationToken = default);

    Task<Fin<GetTradesResponse>> ListAsync(GetTradesRequest request, CancellationToken cancellationToken = default);

    Task<Fin<UpdateTradeResponse>> UpdateAsync(UpdateTradeRequest request, CancellationToken cancellationToken = default);

    Task<Fin<DeleteTradeResponse>> DeleteAsync(DeleteTradeRequest request, CancellationToken cancellationToken = default);
}
