using Acme.Core.Ordering.Persistence.Orders.Mappers;
using OrderItemMapper = Acme.Core.Ordering.Persistence.OrderItems.Mappers.OrderItemMapper;

namespace Acme.Core.Ordering.Persistence;

public static class DboMappers
{
    public static readonly OrderMapper Order = default;
    public static readonly OrderItemMapper OrderItem = default;
}
