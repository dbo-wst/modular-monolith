using Template.Core.Ordering.Contracts.Results;
using Template.Core.Shared.Kernel.Communication;

namespace Template.Core.Ordering.Contracts.Commands;

public sealed record CreateOrderItemCommand(
    Guid? OrderId = null,
    decimal? Quantity = null,
    decimal? Price = null) : ICommand<CreateOrderItemResult>;

