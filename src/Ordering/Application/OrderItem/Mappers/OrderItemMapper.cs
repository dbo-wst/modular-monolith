using Template.Core.Shared.Kernel;

namespace Template.Core.Ordering.Application.OrderItem.Mappers;

public readonly struct OrderItemMapper : IOnewayMap<Template.Core.Ordering.Domain.OrderItem.OrderItem, Integration.Dto.OrderItem>
{
    public Integration.Dto.OrderItem Map(Template.Core.Ordering.Domain.OrderItem.OrderItem left) => new(
        left.OrderItemId.Value,
        left.OrderId?.Value,
        left.Quantity?.Value,
        left.Price?.Value);
}

