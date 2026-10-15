using LanguageExt;
using Acme.Core.Ordering.Contracts.Queries;
using Acme.Core.Ordering.Contracts.Results;
using Acme.Core.BuildingBlocks.Application.Communication;
using Acme.Core.BuildingBlocks.Domain.Functional;

namespace Acme.Core.Ordering.Application.Orders.GetOrder;

internal sealed class GetOrderHandler(IOrderingReadStore readStore) : IQueryHandler<GetOrderQuery, GetOrderResult>
{
    public Task<Fin<GetOrderResult>> HandleAsync(GetOrderQuery request, CancellationToken ct = default) =>
        readStore.GetOrderAsync(request.OrderId, ct)
            .MapAsync(order => new GetOrderResult(order));
}
