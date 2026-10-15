using Template.Core.Ordering.Persistence.Orders.Mappers;
using OrderItemMapper = Template.Core.Ordering.Persistence.OrderItems.Mappers.OrderItemMapper;

namespace Template.Core.Ordering.Persistence;

public static class DboMappers
{
    public static readonly OrderMapper Order = default;
    public static readonly OrderItemMapper OrderItem = default;
}
