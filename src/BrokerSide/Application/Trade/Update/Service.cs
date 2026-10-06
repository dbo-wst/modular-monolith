using LanguageExt;
using Wst.Trios.Core.BrokerSide.Domain.Trade;
using Wst.Trios.Core.BrokerSide.Integration.Requests;
using Wst.Trios.Core.BrokerSide.Integration.Responses;
using Wst.Trios.Core.Shared.Kernel.Communication;
using Wst.Trios.Core.Shared.Kernel.Domain;
using Wst.Trios.Core.Shared.Kernel.Functional;

namespace Wst.Trios.Core.BrokerSide.Application.Trade.Update;

public class Service(Domain.IStore store): IRequestHandler<UpdateTradeRequest, UpdateTradeResponse>
{
    public Task<Fin<UpdateTradeResponse>> HandleAsync(UpdateTradeRequest request, CancellationToken ct = default) =>
        store.Trades.GetByIdAsync(TradeId.From(request.TradeId), ct)
            .MapAsync(TradeAggregate.FromSnapshot)
            .BindAsync(aggregate => aggregate.ChangeDetails(
                OrderId.From(request.OrderId),
                EffectiveDate.From(request.EffectiveDate),
                Quantity.From(request.Quantity),
                Price.From(request.Price),
                Amount.From(request.Amount)))
            .MapAsync(agg => agg.ToSnapshot())
            .BindAsync(snapshot => store.Trades.UpdateAsync(snapshot, ct))
            .BindAsync(snapshot => store.SaveChangesAsync(ct).MapAsync(_ => snapshot))
            .MapAsync(snapshot => new UpdateTradeResponse(DtoMappers.Trade.Map(snapshot)));
}

