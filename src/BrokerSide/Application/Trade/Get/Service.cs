using LanguageExt;
using Wst.Trios.Core.BrokerSide.Domain;
using Wst.Trios.Core.BrokerSide.Integration.Requests;
using Wst.Trios.Core.BrokerSide.Integration.Responses;
using Wst.Trios.Core.Shared.Kernel.Communication;
using Wst.Trios.Core.Shared.Kernel.Functional;
using TradeId = Wst.Trios.Core.BrokerSide.Domain.Trade.TradeId;

namespace Wst.Trios.Core.BrokerSide.Application.Trade.Get;

public class Service(IStore store): IRequestHandler<GetTradeRequest, GetTradeResponse>
{
    public Task<Fin<GetTradeResponse>> HandleAsync(GetTradeRequest request, CancellationToken ct = default) =>
        store.Trades.GetByIdAsync(TradeId.From(request.TradeId), ct)
            .MapAsync(x => new GetTradeResponse(DtoMappers.Trade.Map(x)));
}
