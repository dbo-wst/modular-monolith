using LanguageExt;
using Wst.Trios.Core.BrokerSide.Domain.Trade;
using Wst.Trios.Core.BrokerSide.Integration.Requests;
using Wst.Trios.Core.BrokerSide.Integration.Responses;
using Wst.Trios.Core.Shared.Kernel.Communication;
using Wst.Trios.Core.Shared.Kernel.Domain;
using Wst.Trios.Core.Shared.Kernel.Functional;

namespace Wst.Trios.Core.BrokerSide.Application.Trade.Create;

public class Service(Domain.IStore store) : IRequestHandler<CreateTradeRequest, CreateTradeResponse>
{
    public Task<Fin<CreateTradeResponse>> HandleAsync(CreateTradeRequest request, CancellationToken ct = default) =>
        TradeAggregate.Create(
                OrderId.From(request.OrderId),
                EffectiveDate.From(request.EffectiveDate),
                Quantity.From(request.Quantity),
                Price.From(request.Price),
                Amount.From(request.Amount))
            .AsTask()
            .MapAsync(i => i.ToSnapshot())
            .BindAsync(trade => store.Trades.AddAsync(trade, ct))
            .BindAsync(created => store.SaveChangesAsync(ct).MapAsync(_ => created))
            .MapAsync(created => new CreateTradeResponse(DtoMappers.Trade.Map(created)));
}

