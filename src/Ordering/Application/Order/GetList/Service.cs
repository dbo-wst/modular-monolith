using LanguageExt;
using System.Collections.Immutable;
using Template.Core.Ordering.Domain;
using Template.Core.Ordering.Integration.Requests;
using Template.Core.Ordering.Integration.Responses;
using Template.Core.Shared.Kernel.Communication;
using Template.Core.Shared.Kernel.Functional;

namespace Template.Core.Ordering.Application.Order.GetList;

public class Service(IStore store): IRequestHandler<GetListOrderRequest, GetListOrderResponse>
{
    public Task<Fin<GetListOrderResponse>> HandleAsync(GetListOrderRequest request, CancellationToken ct = default) =>
        store.Orders.GetListAsync(request.PageNumber, request.PageSize, ct)
            .MapAsync(list => list.ToImmutableList().ToFinAsync())
            .MapAsync(list => new GetListOrderResponse(list.Select(DtoMappers.Order.Map).ToImmutableList()));
}
