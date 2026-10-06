using Wst.Trios.Core.BrokerSide.Domain.Trade;

namespace Wst.Trios.Core.BrokerSide.Domain.Test.Trade;

public class TradeIdTests
{
    [Fact]
    public void From_WrapsTheProvidedValue()
    {
        var guid = Guid.NewGuid();

        var id = TradeId.From(guid);

        Assert.Equal(guid, id.Value);
    }

    [Fact]
    public void From_WrapsAnEmptyGuid()
    {
        Assert.Equal(Guid.Empty, TradeId.From(Guid.Empty).Value);
    }

    [Fact]
    public void MaybeCreate_Succeeds()
    {
        var result = TradeId.MaybeCreate(Guid.NewGuid());

        Assert.True(result.IsSucc);
    }

    [Fact]
    public void MaybeCreate_WrapsTheProvidedValue()
    {
        var guid = Guid.NewGuid();

        Assert.Equal(guid, TradeId.MaybeCreate(guid).ThrowIfFail().Value);
    }

    [Fact]
    public void From_Nullable_WithValue_ReturnsId()
    {
        var guid = Guid.NewGuid();

        var id = TradeId.From((Guid?)guid);

        Assert.NotNull(id);
        Assert.Equal(guid, id.Value);
    }

    [Fact]
    public void From_Nullable_WithNull_ReturnsNull()
    {
        Assert.Null(TradeId.From((Guid?)null));
    }

    [Fact]
    public void Equality_TwoIdsWithSameGuid_AreEqual()
    {
        var guid = Guid.NewGuid();

        Assert.Equal(TradeId.From(guid), TradeId.From(guid));
    }

    [Fact]
    public void Equality_TwoIdsWithDifferentGuids_AreNotEqual()
    {
        Assert.NotEqual(TradeId.From(Guid.NewGuid()), TradeId.From(Guid.NewGuid()));
    }
}
