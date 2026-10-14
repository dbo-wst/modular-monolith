using Template.Core.Ordering.Integration.Dto;
using Template.Core.Ordering.Integration.Responses;
using Template.Core.Shared.Kernel.Communication;

namespace Template.Core.Ordering.Integration.Requests;

public sealed record UpdateOrderRequest(
    Guid OrderId,
    string? Number,
    DateOnly? EffectiveDate,
    EStatus? Status) : IRequest<UpdateOrderResponse>;
