using LanguageExt;
using Acme.Core.Ordering.Contracts.Queries;
using Acme.Core.Ordering.Contracts.Results;
using Acme.Core.BuildingBlocks.Application.Communication;
using Acme.Core.BuildingBlocks.Domain.Functional;

namespace Acme.Core.Ordering.Application.OrderItems.GetOrderItem;

internal sealed class GetOrderItemHandler(IOrderingReadStore readStore) : IQueryHandler<GetOrderItemQuery, GetOrderItemResult>
{
    public Task<Fin<GetOrderItemResult>> HandleAsync(GetOrderItemQuery request, CancellationToken ct = default) =>
        readStore.GetOrderItemAsync(request.OrderItemId, ct)
            .MapAsync(orderItem => new GetOrderItemResult(orderItem));
}
