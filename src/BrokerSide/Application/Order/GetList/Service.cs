using LanguageExt;
using System.Collections.Immutable;
using Wst.Trios.Core.BrokerSide.Domain;
using Wst.Trios.Core.BrokerSide.Integration.Requests;
using Wst.Trios.Core.BrokerSide.Integration.Responses;
using Wst.Trios.Core.Shared.Kernel.Communication;
using Wst.Trios.Core.Shared.Kernel.Functional;

namespace Wst.Trios.Core.BrokerSide.Application.Order.GetList;

public class Service(IStore store): IRequestHandler<GetListOrderRequest, GetListOrderResponse>
{
    public Task<Fin<GetListOrderResponse>> HandleAsync(GetListOrderRequest request, CancellationToken ct = default) =>
        store.Orders.GetListAsync(request.PageNumber, request.PageSize, ct)
            .MapAsync(list => list.ToImmutableList().ToFinAsync())
            .MapAsync(list => new GetListOrderResponse(list.Select(DtoMappers.Order.Map).ToImmutableList()));
}
