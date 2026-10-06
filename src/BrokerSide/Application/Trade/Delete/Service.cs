using LanguageExt;
using Wst.Trios.Core.BrokerSide.Domain.Trade;
using Wst.Trios.Core.BrokerSide.Integration.Requests;
using Wst.Trios.Core.BrokerSide.Integration.Responses;
using Wst.Trios.Core.Shared.Kernel.Communication;
using Wst.Trios.Core.Shared.Kernel.Functional;

namespace Wst.Trios.Core.BrokerSide.Application.Trade.Delete;

public class Service(Domain.IStore store): IRequestHandler<DeleteTradeRequest, DeleteTradeResponse>
{

    public Task<Fin<DeleteTradeResponse>> HandleAsync(DeleteTradeRequest request, CancellationToken ct = default)
    {
        var tradeId = TradeId.From(request.TradeId);

        return store.Trades.GetByIdAsync(tradeId, ct)
            .MapAsync(TradeAggregate.FromSnapshot)
            .BindAsync(trade =>
                store.Trades.DeleteAsync(tradeId, ct)
                    .BindAsync(i => trade.Delete())
            )

            .BindAsync(_ => store.SaveChangesAsync(ct))
            .MapAsync(_ => new DeleteTradeResponse());
    }
}
