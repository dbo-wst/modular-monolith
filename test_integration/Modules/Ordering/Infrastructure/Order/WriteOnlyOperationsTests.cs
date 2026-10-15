using LanguageExt.UnsafeValueAccess;
using Template.Core.Ordering.Domain;
using Template.Core.Ordering.TestCompanion;
using Template.Core.Ordering.Domain.ValueObjects;
using OrderId = Template.Core.Ordering.Domain.Orders.OrderId;
using Support = Template.Core.Ordering.TestCompanion.Order.Support;


namespace Template.Core.Ordering.Infrastructure.IntegrationTest.Order;

[Collection(IntegrationTestsFixture.CollectionName)]
public class WriteOnlyOperationsTests(IntegrationTestsFixture fixture)
{
    private readonly Domain.Orders.IOrderRepository _orders = fixture.GetRequiredService<Domain.Orders.IOrderRepository>();
    private readonly Domain.Abstractions.IUnitOfWork _unitOfWork = fixture.GetRequiredService<Domain.Abstractions.IUnitOfWork>();

    [Fact]
    public async Task Test_Order_CreateAsync()
    {
        var expected = Support.Domain.NewOrder();

        await _orders.AddAsync(expected);
        await _unitOfWork.SaveChangesAsync();

        var getById = await _orders.GetByIdAsync(expected.Id);

        Assert.True(getById.IsSucc);
        var fetchedOrder = getById.ToOption().ValueUnsafe();
        Assert.Equal(expected.Id, fetchedOrder.Id);
        Assert.Equal(expected.OrderNumber, fetchedOrder.OrderNumber);
        Assert.Equal(expected.EffectiveDate, fetchedOrder.EffectiveDate);
        Assert.Equal(expected.Status, fetchedOrder.Status);
    }

    [Fact]
    public async Task Test_Order_CreateAsync_ReturnsFailForExistingEntity()
    {
        var order = Support.Domain.NewOrder();

        await _orders.AddAsync(order);
        await _unitOfWork.SaveChangesAsync();

        var actual = await _orders.AddAsync(order);

        Assert.True(actual.IsFail);
        var message = actual.Match(Succ: _ => string.Empty, Fail: error => error.Message);
        Assert.Equal(PersistenceErrors.AlreadyExists(order).Message, message);
    }

    [Fact]
    public async Task Test_Order_UpdateAsync()
    {
        var original = Support.Domain.NewOrder();

        await _orders.AddAsync(original);
        await _unitOfWork.SaveChangesAsync();

        var updated = Domain.Orders.Order.Rehydrate(
            original.Id, OrderNumber.From("ORD-2"), EffectiveDate.From(new DateOnly(2026, 7, 11)), original.Status);
        var actual = await _orders.UpdateAsync(updated);
        await _unitOfWork.SaveChangesAsync();

        Assert.True(actual.IsSucc);
        actual.IfSucc(result => Assert.Equal(updated, result));

        var fetched = await _orders.GetByIdAsync(original.Id);

        Assert.True(fetched.IsSucc);
        var fetchedUpdated = fetched.ToOption().ValueUnsafe();
        Assert.Equal(updated.Id, fetchedUpdated.Id);
        Assert.Equal(updated.OrderNumber, fetchedUpdated.OrderNumber);
        Assert.Equal(updated.EffectiveDate, fetchedUpdated.EffectiveDate);
        Assert.Equal(updated.Status, fetchedUpdated.Status);
    }

    [Fact]
    public async Task Test_Order_UpdateAsync_ReturnsFailForMissingEntity()
    {
        var missing = Support.Domain.NewOrder();

        var actual = await _orders.UpdateAsync(missing);

        Assert.True(actual.IsFail);
        var message = actual.Match(Succ: _ => string.Empty, Fail: error => error.Message);
        Assert.Equal(PersistenceErrors.NotFound(missing).Message, message);
    }

    [Fact]
    public async Task Test_Order_DeleteAsync()
    {
        var order = Support.Domain.NewOrder();

        await _orders.AddAsync(order);
        await _unitOfWork.SaveChangesAsync();

        var actual = await _orders.DeleteAsync(order.Id);
        await _unitOfWork.SaveChangesAsync();

        Assert.True(actual.IsSucc);

        var fetched = await _orders.GetByIdAsync(order.Id);
        Assert.True(fetched.IsFail);
    }

    [Fact]
    public async Task Test_Order_DeleteAsync_ReturnsFailForMissingEntity()
    {
        var missingId = OrderId.From(Guid.NewGuid());

        var actual = await _orders.DeleteAsync(missingId);

        Assert.True(actual.IsFail);
        var message = actual.Match(Succ: _ => string.Empty, Fail: error => error.Message);
        Assert.Equal(PersistenceErrors.NotFound(missingId).Message, message);
    }
}
