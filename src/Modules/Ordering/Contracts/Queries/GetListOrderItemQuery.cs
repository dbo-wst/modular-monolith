using Acme.Core.Ordering.Contracts.Results;
using Acme.Core.BuildingBlocks.Application.Communication;

namespace Acme.Core.Ordering.Contracts.Queries;

public sealed record GetListOrderItemQuery(int PageNumber, int PageSize): IQuery<GetListOrderItemResult>;


