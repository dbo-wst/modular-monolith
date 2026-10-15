using Acme.Core.Ordering.Contracts.Results;
using Acme.Core.BuildingBlocks.Application.Communication;

namespace Acme.Core.Ordering.Contracts.Queries;

public sealed record GetListOrderQuery(int PageNumber, int PageSize): IQuery<GetListOrderResult>;


