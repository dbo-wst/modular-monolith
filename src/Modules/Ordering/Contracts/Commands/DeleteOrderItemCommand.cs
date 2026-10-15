using Template.Core.Ordering.Contracts.Results;
using Template.Core.Shared.Kernel.Communication;

namespace Template.Core.Ordering.Contracts.Commands;

public sealed record DeleteOrderItemCommand(Guid OrderItemId): ICommand<DeleteOrderItemResult>;


