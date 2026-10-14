using LanguageExt;
using System.Collections.Immutable;
using Template.Core.Ordering.Domain;
using Template.Core.Ordering.Integration.Requests;
using Template.Core.Ordering.Integration.Responses;
using Template.Core.Shared.Kernel.Communication;
using Template.Core.Shared.Kernel.Functional;

namespace Template.Core.Ordering.Application.OrderItem.GetList;

public class Service(IStore store): IRequestHandler<GetListOrderItemRequest, GetListOrderItemResponse>
{
    public Task<Fin<GetListOrderItemResponse>> HandleAsync(GetListOrderItemRequest request, CancellationToken ct = default) =>
        store.OrderItems.GetListAsync(request.PageNumber, request.PageSize, ct)
            .MapAsync(list => list.ToImmutableList().ToFinAsync())
            .MapAsync(list => new GetListOrderItemResponse(list.Select(DtoMappers.OrderItem.Map).ToImmutableList()));
}
