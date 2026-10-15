using Template.Core.Ordering.Contracts.Results;
using Template.Core.Shared.Kernel.Communication;

namespace Template.Core.Ordering.Contracts.Queries;

public sealed record GetOrderItemQuery(Guid OrderItemId) : IQuery<GetOrderItemResult>;


