using Acme.Core.Ordering.Domain.Orders;

namespace Acme.Core.Ordering.Domain.Test.Order;

public class OrderIdTests
{
    [Fact]
    public void From_WrapsTheProvidedValue()
    {
        var guid = Guid.NewGuid();

        var id = OrderId.From(guid);

        Assert.Equal(guid, id.Value);
    }

    [Fact]
    public void From_WrapsAnEmptyGuid()
    {
        Assert.Equal(Guid.Empty, OrderId.From(Guid.Empty).Value);
    }

    [Fact]
    public void MaybeCreate_Succeeds()
    {
        var result = OrderId.MaybeCreate(Guid.NewGuid());

        Assert.True(result.IsSucc);
    }

    [Fact]
    public void MaybeCreate_WrapsTheProvidedValue()
    {
        var guid = Guid.NewGuid();

        Assert.Equal(guid, OrderId.MaybeCreate(guid).ThrowIfFail().Value);
    }

    [Fact]
    public void From_Nullable_WithValue_ReturnsId()
    {
        var guid = Guid.NewGuid();

        var id = OrderId.From((Guid?)guid);

        Assert.NotNull(id);
        Assert.Equal(guid, id.Value);
    }

    [Fact]
    public void From_Nullable_WithNull_ReturnsNull()
    {
        Assert.Null(OrderId.From((Guid?)null));
    }

    [Fact]
    public void Equality_TwoIdsWithSameGuid_AreEqual()
    {
        var guid = Guid.NewGuid();

        Assert.Equal(OrderId.From(guid), OrderId.From(guid));
    }

    [Fact]
    public void Equality_TwoIdsWithDifferentGuids_AreNotEqual()
    {
        Assert.NotEqual(OrderId.From(Guid.NewGuid()), OrderId.From(Guid.NewGuid()));
    }
}
