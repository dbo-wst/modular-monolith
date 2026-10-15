using Template.Core.Ordering.Domain.Orders;
using Template.Core.Ordering.Domain.OrderItems;
using Template.Core.Ordering.Domain.ValueObjects;

namespace Template.Core.Ordering.Persistence.IntegrationTest.OrderItems;

public class ReadOnlyOperationsTests
{
    [Fact]
    public void OrderItemRoundTripPreservesValues()
    {
        var orderItem = Domain.OrderItems.OrderItem.Rehydrate(
            OrderItemId.From(Guid.NewGuid()),
            OrderId.From(Guid.NewGuid()),
            Quantity.From(100.5m),
            Price.From(25.75m));

        var dbo = DboMappers.OrderItem.Map(orderItem);
        var roundTripped = DboMappers.OrderItem.Map(dbo);

        Assert.Equal(orderItem.Id, roundTripped.Id);
        Assert.Equal(orderItem.OrderId, roundTripped.OrderId);
        Assert.Equal(orderItem.Quantity, roundTripped.Quantity);
        Assert.Equal(orderItem.Price, roundTripped.Price);
    }
}
