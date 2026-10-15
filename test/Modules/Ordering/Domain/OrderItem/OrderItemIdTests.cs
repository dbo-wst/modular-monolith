using Template.Core.Ordering.Domain.OrderItems;

namespace Template.Core.Ordering.Domain.Test.OrderItem;

public class OrderItemIdTests
{
    [Fact]
    public void From_WrapsTheProvidedValue()
    {
        var guid = Guid.NewGuid();

        var id = OrderItemId.From(guid);

        Assert.Equal(guid, id.Value);
    }

    [Fact]
    public void From_WrapsAnEmptyGuid()
    {
        Assert.Equal(Guid.Empty, OrderItemId.From(Guid.Empty).Value);
    }

    [Fact]
    public void MaybeCreate_Succeeds()
    {
        var result = OrderItemId.MaybeCreate(Guid.NewGuid());

        Assert.True(result.IsSucc);
    }

    [Fact]
    public void MaybeCreate_WrapsTheProvidedValue()
    {
        var guid = Guid.NewGuid();

        Assert.Equal(guid, OrderItemId.MaybeCreate(guid).ThrowIfFail().Value);
    }

    [Fact]
    public void From_Nullable_WithValue_ReturnsId()
    {
        var guid = Guid.NewGuid();

        var id = OrderItemId.From((Guid?)guid);

        Assert.NotNull(id);
        Assert.Equal(guid, id.Value);
    }

    [Fact]
    public void From_Nullable_WithNull_ReturnsNull()
    {
        Assert.Null(OrderItemId.From((Guid?)null));
    }

    [Fact]
    public void Equality_TwoIdsWithSameGuid_AreEqual()
    {
        var guid = Guid.NewGuid();

        Assert.Equal(OrderItemId.From(guid), OrderItemId.From(guid));
    }

    [Fact]
    public void Equality_TwoIdsWithDifferentGuids_AreNotEqual()
    {
        Assert.NotEqual(OrderItemId.From(Guid.NewGuid()), OrderItemId.From(Guid.NewGuid()));
    }
}
