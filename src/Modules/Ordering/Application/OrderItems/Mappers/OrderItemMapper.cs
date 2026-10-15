using Template.Core.BuildingBlocks.Domain;

namespace Template.Core.Ordering.Application.OrderItems.Mappers;

public readonly struct OrderItemMapper : IOnewayMap<Template.Core.Ordering.Domain.OrderItems.OrderItem, Contracts.Dto.OrderItem>
{
    public Contracts.Dto.OrderItem Map(Template.Core.Ordering.Domain.OrderItems.OrderItem left) => new(
        left.Id.Value,
        left.OrderId.Value,
        left.Quantity.Value,
        left.Price.Value);
}

