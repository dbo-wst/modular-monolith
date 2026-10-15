using Acme.Core.Ordering.Contracts.Results;
using Acme.Core.BuildingBlocks.Application.Communication;

namespace Acme.Core.Ordering.Contracts.Commands;

public sealed record UpdateOrderItemCommand(
    Guid OrderItemId,
    Guid? OrderId = null,
    decimal? Quantity = null,
    decimal? Price = null) : ICommand<UpdateOrderItemResult>;

