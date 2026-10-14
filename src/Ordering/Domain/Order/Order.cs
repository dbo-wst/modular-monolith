using Template.Core.Shared.Kernel.Domain;

namespace Template.Core.Ordering.Domain.Order;

public record Order(
    OrderId OrderId,
    OrderNumber? OrderNumber,
    EffectiveDate? EffectiveDate,
    Status? Status);
