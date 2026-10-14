using Template.Core.Ordering.Domain.OrderItem;
using Template.Core.Shared.Kernel.Domain;

namespace Template.Core.Ordering.Persistence.IntegrationTest.OrderItem;

public class ReadOnlyOperationsTests
{
    [Fact]
    public void OrderItemRoundTripPreservesValues()
    {
        var orderItem = new Domain.OrderItem.OrderItem(
            OrderItemId.From(Guid.NewGuid()),
            OrderId.From(Guid.NewGuid()),
            Quantity.From(100.5m),
            Price.From(25.75m));

        var dbo = DboMappers.OrderItem.Map(orderItem);
        var roundTripped = DboMappers.OrderItem.Map(dbo);

        Assert.Equal(orderItem, roundTripped);
    }
}
