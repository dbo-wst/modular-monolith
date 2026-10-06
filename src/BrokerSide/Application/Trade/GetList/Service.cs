using LanguageExt;
using System.Collections.Immutable;
using Wst.Trios.Core.BrokerSide.Domain;
using Wst.Trios.Core.BrokerSide.Integration.Requests;
using Wst.Trios.Core.BrokerSide.Integration.Responses;
using Wst.Trios.Core.Shared.Kernel.Communication;
using Wst.Trios.Core.Shared.Kernel.Functional;

namespace Wst.Trios.Core.BrokerSide.Application.Trade.GetList;

public class Service(IStore store): IRequestHandler<GetListTradeRequest, GetListTradeResponse>
{
    public Task<Fin<GetListTradeResponse>> HandleAsync(GetListTradeRequest request, CancellationToken ct = default) =>
        store.Trades.GetListAsync(request.PageNumber, request.PageSize, ct)
            .MapAsync(list => list.ToImmutableList().ToFinAsync())
            .MapAsync(list => new GetListTradeResponse(list.Select(DtoMappers.Trade.Map).ToImmutableList()));
}
