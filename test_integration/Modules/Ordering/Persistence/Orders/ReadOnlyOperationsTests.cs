using LanguageExt.UnsafeValueAccess;
using Template.Core.Ordering.Domain;
using Template.Core.Ordering.TestCompanion;
using OrderId = Template.Core.Ordering.Domain.Orders.OrderId;
using Support = Template.Core.Ordering.TestCompanion.Order.Support;

namespace Template.Core.Ordering.Persistence.IntegrationTest.Orders;

[Collection(IntegrationTestsFixture.CollectionName)]
public class ReadOnlyOperationsTests(IntegrationTestsFixture fixture)
{
    private readonly Domain.Orders.IOrderRepository _orders = fixture.GetRequiredService<Domain.Orders.IOrderRepository>();
    private readonly BuildingBlocks.Domain.IUnitOfWork _unitOfWork = fixture.GetRequiredService<BuildingBlocks.Domain.IUnitOfWork>();

    [Fact]
    public async Task Test_GetByIdAsync_ReturnsFailForMissingEntity()
    {
        var missingId = OrderId.From(Guid.NewGuid());

        var actual = await _orders.GetByIdAsync(missingId);

        Assert.True(actual.IsFail);
    }

    [Fact]
    public async Task Test_GetByIdAsync_ReturnsPersistedEntity()
    {
        var expected = Support.Domain.NewOrder();
        await _orders.AddAsync(expected);
        await _unitOfWork.SaveChangesAsync();

        var actual = await _orders.GetByIdAsync(expected.Id);

        Assert.True(actual.IsSucc);
        var fetchedOrder = actual.ToOption().ValueUnsafe();
        Assert.Equal(expected.Id, fetchedOrder.Id);
        Assert.Equal(expected.OrderNumber, fetchedOrder.OrderNumber);
        Assert.Equal(expected.EffectiveDate, fetchedOrder.EffectiveDate);
        Assert.Equal(expected.Status, fetchedOrder.Status);
    }

}
