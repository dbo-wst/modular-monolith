using LanguageExt;
using Template.Core.Ordering.Domain.OrderItem;
using Template.Core.Ordering.Integration.Requests;
using Template.Core.Ordering.Integration.Responses;
using Template.Core.Shared.Kernel.Communication;
using Template.Core.Shared.Kernel.Domain;
using Template.Core.Shared.Kernel.Functional;

namespace Template.Core.Ordering.Application.OrderItem.Update;

public class Service(Domain.IStore store): IRequestHandler<UpdateOrderItemRequest, UpdateOrderItemResponse>
{
    public Task<Fin<UpdateOrderItemResponse>> HandleAsync(UpdateOrderItemRequest request, CancellationToken ct = default) =>
        store.OrderItems.GetByIdAsync(OrderItemId.From(request.OrderItemId), ct)
            .MapAsync(OrderItemAggregate.FromSnapshot)
            .BindAsync(aggregate => aggregate.ChangeDetails(
                OrderId.From(request.OrderId),
                Quantity.From(request.Quantity),
                Price.From(request.Price)))
            .MapAsync(agg => agg.ToSnapshot())
            .BindAsync(snapshot => store.OrderItems.UpdateAsync(snapshot, ct))
            .BindAsync(snapshot => store.SaveChangesAsync(ct).MapAsync(_ => snapshot))
            .MapAsync(snapshot => new UpdateOrderItemResponse(DtoMappers.OrderItem.Map(snapshot)));
}

