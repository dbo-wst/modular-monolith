using LanguageExt;
using Template.Core.Ordering.Domain.OrderItem;
using Template.Core.Ordering.Integration.Requests;
using Template.Core.Ordering.Integration.Responses;
using Template.Core.Shared.Kernel.Communication;
using Template.Core.Shared.Kernel.Domain;
using Template.Core.Shared.Kernel.Functional;

namespace Template.Core.Ordering.Application.OrderItem.Create;

public class Service(Domain.IStore store) : IRequestHandler<CreateOrderItemRequest, CreateOrderItemResponse>
{
    public Task<Fin<CreateOrderItemResponse>> HandleAsync(CreateOrderItemRequest request, CancellationToken ct = default) =>
        OrderItemAggregate.Create(
                OrderId.From(request.OrderId),
                Quantity.From(request.Quantity),
                Price.From(request.Price))
            .AsTask()
            .MapAsync(i => i.ToSnapshot())
            .BindAsync(orderItem => store.OrderItems.AddAsync(orderItem, ct))
            .BindAsync(created => store.SaveChangesAsync(ct).MapAsync(_ => created))
            .MapAsync(created => new CreateOrderItemResponse(DtoMappers.OrderItem.Map(created)));
}

