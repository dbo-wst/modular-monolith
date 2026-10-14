using LanguageExt;
using Template.Core.Ordering.Domain.OrderItem;
using Template.Core.Ordering.Integration.Requests;
using Template.Core.Ordering.Integration.Responses;
using Template.Core.Shared.Kernel.Communication;
using Template.Core.Shared.Kernel.Functional;

namespace Template.Core.Ordering.Application.OrderItem.Delete;

public class Service(Domain.IStore store): IRequestHandler<DeleteOrderItemRequest, DeleteOrderItemResponse>
{

    public Task<Fin<DeleteOrderItemResponse>> HandleAsync(DeleteOrderItemRequest request, CancellationToken ct = default)
    {
        var orderItemId = OrderItemId.From(request.OrderItemId);

        return store.OrderItems.GetByIdAsync(orderItemId, ct)
            .MapAsync(OrderItemAggregate.FromSnapshot)
            .BindAsync(orderItem =>
                store.OrderItems.DeleteAsync(orderItemId, ct)
                    .BindAsync(i => orderItem.Delete())
            )

            .BindAsync(_ => store.SaveChangesAsync(ct))
            .MapAsync(_ => new DeleteOrderItemResponse());
    }
}
