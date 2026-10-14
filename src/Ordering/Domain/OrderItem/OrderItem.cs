using Template.Core.Shared.Kernel.Domain;

namespace Template.Core.Ordering.Domain.OrderItem;

public record OrderItem(
    OrderItemId OrderItemId,
    OrderId? OrderId,
    Quantity? Quantity,
    Price? Price);
