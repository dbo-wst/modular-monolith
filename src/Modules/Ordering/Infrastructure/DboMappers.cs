using Template.Core.Ordering.Infrastructure.Order.Mappers;
using OrderItemMapper = Template.Core.Ordering.Infrastructure.OrderItem.Mappers.OrderItemMapper;

namespace Template.Core.Ordering.Infrastructure;

public static class DboMappers
{
    public static readonly OrderMapper Order = default;
    public static readonly OrderItemMapper OrderItem = default;
}
