using LanguageExt;
using System.Collections.Immutable;
using Acme.Core.Ordering.Contracts.Queries;
using Acme.Core.Ordering.Contracts.Results;
using Acme.Core.BuildingBlocks.Application.Communication;

namespace Acme.Core.Ordering.Application.Orders.GetListOrder;

internal sealed class GetListOrderHandler(IOrderingReadStore readStore) : IQueryHandler<GetListOrderQuery, GetListOrderResult>
{
    public async Task<Fin<GetListOrderResult>> HandleAsync(GetListOrderQuery request, CancellationToken ct = default)
    {
        var orders = await readStore.GetOrdersAsync(request.PageNumber, request.PageSize, ct);
        return Fin<GetListOrderResult>.Succ(new GetListOrderResult(orders.ToImmutableList()));
    }
}
