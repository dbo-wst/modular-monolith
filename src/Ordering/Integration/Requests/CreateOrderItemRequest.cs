using Template.Core.Ordering.Integration.Responses;
using Template.Core.Shared.Kernel.Communication;

namespace Template.Core.Ordering.Integration.Requests;

public sealed record CreateOrderItemRequest(
    Guid? OrderId = null,
    decimal? Quantity = null,
    decimal? Price = null) : IRequest<CreateOrderItemResponse>;

