using Acme.Core.Ordering.Contracts.Results;
using Acme.Core.BuildingBlocks.Application.Communication;

namespace Acme.Core.Ordering.Contracts.Commands;

public sealed record DeleteOrderItemCommand(Guid OrderItemId): ICommand<DeleteOrderItemResult>;


