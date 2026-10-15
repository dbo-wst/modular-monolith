using LanguageExt;
using System.Collections.Immutable;
using Template.Core.Ordering.Contracts.Queries;
using Template.Core.Ordering.Contracts.Results;
using Template.Core.BuildingBlocks.Application.Communication;

namespace Template.Core.Ordering.Application.Orders.GetListOrder;

internal sealed class GetListOrderHandler(IOrderingReadStore readStore) : IQueryHandler<GetListOrderQuery, GetListOrderResult>
{
    public async Task<Fin<GetListOrderResult>> HandleAsync(GetListOrderQuery request, CancellationToken ct = default)
    {
        var orders = await readStore.GetOrdersAsync(request.PageNumber, request.PageSize, ct);
        return Fin<GetListOrderResult>.Succ(new GetListOrderResult(orders.ToImmutableList()));
    }
}
