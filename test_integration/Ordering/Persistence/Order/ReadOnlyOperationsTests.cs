using LanguageExt.UnsafeValueAccess;
using Template.Core.Ordering.Domain;
using Template.Core.Ordering.TestCompanion;
using OrderId = Template.Core.Ordering.Domain.Order.OrderId;
using Support = Template.Core.Ordering.TestCompanion.Order.Support;

namespace Template.Core.Ordering.Persistence.IntegrationTest.Order;

[Collection(IntegrationTestsFixture.CollectionName)]
public class ReadOnlyOperationsTests(IntegrationTestsFixture fixture)
{
    private readonly IStore _store = fixture.GetRequiredService<IStore>();

    [Fact]
    public async Task Test_GetByIdAsync_ReturnsFailForMissingEntity()
    {
        var missingId = OrderId.From(Guid.NewGuid());

        var actual = await _store.Orders.GetByIdAsync(missingId);

        Assert.True(actual.IsFail);
    }

    [Fact]
    public async Task Test_GetByIdAsync_ReturnsPersistedEntity()
    {
        var expected = Support.Domain.NewOrder();
        await _store.Orders.AddAsync(expected);
        await _store.SaveChangesAsync();

        var actual = await _store.Orders.GetByIdAsync(expected.OrderId);

        Assert.True(actual.IsSucc);
        Assert.Equal(expected, actual.ToOption().ValueUnsafe());
    }

    [Fact]
    public async Task Test_GetListAsync_ReturnsPersistedEntities()
    {
        var first = Support.Domain.NewOrder();
        var second = Support.Domain.NewOrder();
        await _store.Orders.AddAsync(first);
        await _store.Orders.AddAsync(second);
        await _store.SaveChangesAsync();

        var actual = await _store.Orders.GetListAsync(1, 100);

        Assert.Contains(first, actual);
        Assert.Contains(second, actual);
    }

    [Fact]
    public async Task Test_GetListAsync_PageBeyondDataReturnsEmpty()
    {
        var order = Support.Domain.NewOrder();
        await _store.Orders.AddAsync(order);
        await _store.SaveChangesAsync();

        // A page far past the persisted rows yields no results.
        var actual = await _store.Orders.GetListAsync(int.MaxValue, 100);

        Assert.Empty(actual);
    }
}
