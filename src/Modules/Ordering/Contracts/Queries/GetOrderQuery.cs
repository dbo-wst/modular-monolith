using Template.Core.Ordering.Contracts.Results;
using Template.Core.BuildingBlocks.Application.Communication;

namespace Template.Core.Ordering.Contracts.Queries;

public sealed record GetOrderQuery(Guid OrderId) : IQuery<GetOrderResult>;


