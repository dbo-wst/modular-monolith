using Template.Core.Ordering.Contracts.Results;
using Template.Core.BuildingBlocks.Application.Communication;

namespace Template.Core.Ordering.Contracts.Commands;

public sealed record DeleteOrderCommand(Guid OrderId): ICommand<DeleteOrderResult>;


