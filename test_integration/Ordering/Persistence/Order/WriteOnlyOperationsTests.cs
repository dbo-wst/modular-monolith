using LanguageExt.UnsafeValueAccess;
using Template.Core.Ordering.Domain;
using Template.Core.Ordering.TestCompanion;
using Template.Core.Shared.Kernel.Domain;
using OrderId = Template.Core.Ordering.Domain.Order.OrderId;
using Support = Template.Core.Ordering.TestCompanion.Order.Support;


namespace Template.Core.Ordering.Persistence.IntegrationTest.Order;

[Collection(IntegrationTestsFixture.CollectionName)]
public class WriteOnlyOperationsTests(IntegrationTestsFixture fixture)
{
    private readonly IStore _store = fixture.GetRequiredService<IStore>();

    [Fact]
    public async Task Test_Order_CreateAsync()
    {
        var expected = Support.Domain.NewOrder();

        await _store.Orders.AddAsync(expected);
        await _store.SaveChangesAsync();

        var getById = await _store.Orders.GetByIdAsync(expected.OrderId);

        Assert.True(getById.IsSucc);
        Assert.Equal(expected, getById.ToOption().ValueUnsafe());
    }

    [Fact]
    public async Task Test_Order_CreateAsync_ReturnsFailForExistingEntity()
    {
        var order = Support.Domain.NewOrder();

        await _store.Orders.AddAsync(order);
        await _store.SaveChangesAsync();

        var actual = await _store.Orders.AddAsync(order);

        Assert.True(actual.IsFail);
        var message = actual.Match(Succ: _ => string.Empty, Fail: error => error.Message);
        Assert.Equal(PersistenceErrors.AlreadyExists(order).Message, message);
    }

    [Fact]
    public async Task Test_Order_UpdateAsync()
    {
        var original = Support.Domain.NewOrder();

        await _store.Orders.AddAsync(original);
        await _store.SaveChangesAsync();

        var updated = original with
        {
            EffectiveDate = EffectiveDate.From(new DateOnly(2026, 7, 11)),
            OrderNumber = OrderNumber.From("ORD-2")
        };
        var actual = await _store.Orders.UpdateAsync(updated);
        await _store.SaveChangesAsync();

        Assert.True(actual.IsSucc);
        actual.IfSucc(result => Assert.Equal(updated, result));

        var fetched = await _store.Orders.GetByIdAsync(original.OrderId);

        Assert.True(fetched.IsSucc);
        Assert.Equal(updated, fetched.ToOption().ValueUnsafe());
    }

    [Fact]
    public async Task Test_Order_UpdateAsync_ReturnsFailForMissingEntity()
    {
        var missing = Support.Domain.NewOrder();

        var actual = await _store.Orders.UpdateAsync(missing);

        Assert.True(actual.IsFail);
        var message = actual.Match(Succ: _ => string.Empty, Fail: error => error.Message);
        Assert.Equal(PersistenceErrors.NotFound(missing).Message, message);
    }

    [Fact]
    public async Task Test_Order_DeleteAsync()
    {
        var order = Support.Domain.NewOrder();

        await _store.Orders.AddAsync(order);
        await _store.SaveChangesAsync();

        var actual = await _store.Orders.DeleteAsync(order.OrderId);
        await _store.SaveChangesAsync();

        Assert.True(actual.IsSucc);

        var fetched = await _store.Orders.GetByIdAsync(order.OrderId);
        Assert.True(fetched.IsFail);
    }

    [Fact]
    public async Task Test_Order_DeleteAsync_ReturnsFailForMissingEntity()
    {
        var missingId = OrderId.From(Guid.NewGuid());

        var actual = await _store.Orders.DeleteAsync(missingId);

        Assert.True(actual.IsFail);
        var message = actual.Match(Succ: _ => string.Empty, Fail: error => error.Message);
        Assert.Equal(PersistenceErrors.NotFound(missingId).Message, message);
    }
}
