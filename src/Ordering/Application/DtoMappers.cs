using Template.Core.Ordering.Application.Order.Mappers;
using OrderItemMapper = Template.Core.Ordering.Application.OrderItem.Mappers.OrderItemMapper;

namespace Template.Core.Ordering.Application;

public static class DtoMappers
{
    public static readonly OrderMapper Order = default;
    public static readonly StatusMapper Status = default;
    public static readonly OrderItemMapper OrderItem = default;
}
