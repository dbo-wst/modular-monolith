using Template.Core.Ordering.Contracts.Results;
using Template.Core.BuildingBlocks.Application.Communication;

namespace Template.Core.Ordering.Contracts.Queries;

public sealed record GetOrderItemQuery(Guid OrderItemId) : IQuery<GetOrderItemResult>;


