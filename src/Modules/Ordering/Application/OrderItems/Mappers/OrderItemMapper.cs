using Acme.Core.BuildingBlocks.Domain;

namespace Acme.Core.Ordering.Application.OrderItems.Mappers;

public readonly struct OrderItemMapper : IOnewayMap<Acme.Core.Ordering.Domain.OrderItems.OrderItem, Contracts.Dto.OrderItem>
{
    public Contracts.Dto.OrderItem Map(Acme.Core.Ordering.Domain.OrderItems.OrderItem left) => new(
        left.Id.Value,
        left.OrderId.Value,
        left.Quantity.Value,
        left.Price.Value);
}

