using LanguageExt;
using Template.Core.Ordering.Contracts.Queries;
using Template.Core.Ordering.Contracts.Results;
using Template.Core.Shared.Kernel.Communication;
using Template.Core.Shared.Kernel.Functional;

namespace Template.Core.Ordering.Application.Orders.GetOrder;

internal sealed class GetOrderHandler(IOrderingReadStore readStore) : IQueryHandler<GetOrderQuery, GetOrderResult>
{
    public Task<Fin<GetOrderResult>> HandleAsync(GetOrderQuery request, CancellationToken ct = default) =>
        readStore.GetOrderAsync(request.OrderId, ct)
            .MapAsync(order => new GetOrderResult(order));
}
