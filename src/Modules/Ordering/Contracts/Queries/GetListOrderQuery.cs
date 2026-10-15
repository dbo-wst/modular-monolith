using Template.Core.Ordering.Contracts.Results;
using Template.Core.BuildingBlocks.Application.Communication;

namespace Template.Core.Ordering.Contracts.Queries;

public sealed record GetListOrderQuery(int PageNumber, int PageSize): IQuery<GetListOrderResult>;


