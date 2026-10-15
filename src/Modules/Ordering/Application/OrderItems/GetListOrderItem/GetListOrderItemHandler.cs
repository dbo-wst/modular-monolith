using LanguageExt;
using System.Collections.Immutable;
using Template.Core.Ordering.Contracts.Queries;
using Template.Core.Ordering.Contracts.Results;
using Template.Core.Shared.Kernel.Communication;

namespace Template.Core.Ordering.Application.OrderItems.GetListOrderItem;

internal sealed class GetListOrderItemHandler(IOrderingReadStore readStore) : IQueryHandler<GetListOrderItemQuery, GetListOrderItemResult>
{
    public async Task<Fin<GetListOrderItemResult>> HandleAsync(GetListOrderItemQuery request, CancellationToken ct = default)
    {
        var orderItems = await readStore.GetOrderItemsAsync(request.PageNumber, request.PageSize, ct);
        return Fin<GetListOrderItemResult>.Succ(new GetListOrderItemResult(orderItems.ToImmutableList()));
    }
}
