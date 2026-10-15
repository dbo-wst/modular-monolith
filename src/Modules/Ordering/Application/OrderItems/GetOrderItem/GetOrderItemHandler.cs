using LanguageExt;
using Template.Core.Ordering.Contracts.Queries;
using Template.Core.Ordering.Contracts.Results;
using Template.Core.Shared.Kernel.Communication;
using Template.Core.Shared.Kernel.Functional;

namespace Template.Core.Ordering.Application.OrderItems.GetOrderItem;

internal sealed class GetOrderItemHandler(IOrderingReadStore readStore) : IQueryHandler<GetOrderItemQuery, GetOrderItemResult>
{
    public Task<Fin<GetOrderItemResult>> HandleAsync(GetOrderItemQuery request, CancellationToken ct = default) =>
        readStore.GetOrderItemAsync(request.OrderItemId, ct)
            .MapAsync(orderItem => new GetOrderItemResult(orderItem));
}
