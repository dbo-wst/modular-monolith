using Template.Core.Ordering.Domain.Orders;
using LanguageExt;
using Template.Core.Ordering.Domain.OrderItems;
using Template.Core.Ordering.Contracts.Commands;
using Template.Core.Ordering.Contracts.Results;
using Template.Core.Shared.Kernel.Communication;
using Template.Core.Ordering.Domain.ValueObjects;
using Template.Core.Shared.Kernel.Errors;
using Template.Core.Shared.Kernel.Functional;

namespace Template.Core.Ordering.Application.OrderItems.CreateOrderItem;

internal sealed class CreateOrderItemHandler(Domain.OrderItems.IOrderItemRepository orderItems, Domain.Abstractions.IUnitOfWork unitOfWork) : ICommandHandler<CreateOrderItemCommand, CreateOrderItemResult>
{
    public Task<Fin<CreateOrderItemResult>> HandleAsync(CreateOrderItemCommand request, CancellationToken ct = default) =>
        Build(request)
            .AsTask()
            .BindAsync(orderItem => orderItems.AddAsync(orderItem, ct))
            .BindAsync(created => unitOfWork.SaveChangesAsync(ct).MapAsync(_ => created))
            .MapAsync(created => new CreateOrderItemResult(DtoMappers.OrderItem.Map(created)));

    private static Fin<OrderItem> Build(CreateOrderItemCommand request)
    {
        var orderId = OrderId.From(request.OrderId);
        var quantity = Quantity.From(request.Quantity);
        var price = Price.From(request.Price);

        return orderId is null || quantity is null || price is null
            ? Fin<OrderItem>.Fail(ValidationErrors.ValidationError("Order id, quantity and price are required."))
            : OrderItem.Create(orderId, quantity, price);
    }
}

