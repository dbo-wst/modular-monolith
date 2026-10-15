using Template.Core.Ordering.Contracts.Results;
using Template.Core.Shared.Kernel.Communication;

namespace Template.Core.Ordering.Contracts.Commands;

public sealed record UpdateOrderItemCommand(
    Guid OrderItemId,
    Guid? OrderId = null,
    decimal? Quantity = null,
    decimal? Price = null) : ICommand<UpdateOrderItemResult>;

