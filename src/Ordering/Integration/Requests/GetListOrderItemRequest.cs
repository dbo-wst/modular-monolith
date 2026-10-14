using Template.Core.Ordering.Integration.Responses;
using Template.Core.Shared.Kernel.Communication;

namespace Template.Core.Ordering.Integration.Requests;

public sealed record GetListOrderItemRequest(int PageNumber, int PageSize): IRequest<GetListOrderItemResponse>;


