using Template.Core.Ordering.Domain.Orders;
using LanguageExt;
using Template.Core.BuildingBlocks.Domain;
using Template.Core.Ordering.Domain.OrderItems;
using Template.Core.Ordering.Contracts.Commands;
using Template.Core.Ordering.Contracts.Results;
using Template.Core.BuildingBlocks.Application.Communication;
using Template.Core.Ordering.Domain.ValueObjects;
using Template.Core.BuildingBlocks.Domain.Errors;
using Template.Core.BuildingBlocks.Domain.Functional;

namespace Template.Core.Ordering.Application.OrderItems.CreateOrderItem;

internal sealed class CreateOrderItemHandler(Domain.OrderItems.IOrderItemRepository orderItems, IUnitOfWork unitOfWork) : ICommandHandler<CreateOrderItemCommand, CreateOrderItemResult>
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
            ? Fin<OrderItem>.Fail(ValidationError.New("Order id, quantity and price are required."))
            : OrderItem.Create(orderId, quantity, price);
    }
}

