using LanguageExt.UnsafeValueAccess;
using Template.Core.Ordering.Persistence;
using Template.Core.Ordering.TestCompanion;
using CreateOrderRequest = Template.Core.Ordering.Application.Order.Create;
using Dto = Template.Core.Ordering.Integration.Dto;
using EStatus = Template.Core.Ordering.Integration.Dto.EStatus;
using GetOrderRequest = Template.Core.Ordering.Application.Order.Get;
using OrderId = Template.Core.Ordering.Domain.Order.OrderId;
using UpdateOrderRequest = Template.Core.Ordering.Application.Order.Update;

namespace Template.Core.Ordering.Application.IntegrationTest.Order.Update;

[Collection(IntegrationTestsFixture.CollectionName)]
public class ServiceTests(IntegrationTestsFixture fixture)
{
    private readonly CreateOrderRequest.Service _createService = new(new Store(fixture.GetRequiredService<IOrderingDbContextFactory>()));
    private readonly UpdateOrderRequest.Service _updateService = new(new Store(fixture.GetRequiredService<IOrderingDbContextFactory>()));
    private readonly GetOrderRequest.Service _getService = new(new Store(fixture.GetRequiredService<IOrderingDbContextFactory>()));

    private async Task<Dto.Order> CreateBaselineAsync()
    {
        var maybeCreated = await _createService.HandleAsync(new Integration.Requests.CreateOrderRequest(
            "ORD-1", new DateOnly(2026, 7, 10)));
        Assert.True(maybeCreated.IsSucc);
        var created = maybeCreated.ToOption().ValueUnsafe();
        Assert.NotNull(created);
        return created.Order;
    }

    private async Task<Dto.Order> UpdateAsync(Integration.Requests.UpdateOrderRequest request)
    {
        var maybeUpdated = await _updateService.HandleAsync(request);
        Assert.True(maybeUpdated.IsSucc);
        var updated = maybeUpdated.ToOption().ValueUnsafe();
        Assert.NotNull(updated);
        return updated.Order;
    }

    private async Task AssertPersistedAsync(Dto.Order expected)
    {
        var maybeOrder = await _getService.HandleAsync(new Integration.Requests.GetOrderRequest(expected.OrderId));
        Assert.True(maybeOrder.IsSucc);
        var actual = maybeOrder.ToOption().ValueUnsafe();
        Assert.NotNull(actual);
        Assert.Equal(expected, actual.Order);
    }

    [Fact]
    public async Task Test_Update_Get_Success()
    {
        var baseline = await CreateBaselineAsync();

        var newEffectiveDate = new DateOnly(2026, 7, 11);
        var updated = await UpdateAsync(new Integration.Requests.UpdateOrderRequest(
            baseline.OrderId, "ORD-2", newEffectiveDate, EStatus.Completed));

        Assert.Equal(baseline.OrderId, updated.OrderId);
        Assert.Equal("ORD-2", updated.OrderNumber);
        Assert.Equal(newEffectiveDate, updated.EffectiveDate);
        Assert.Equal(EStatus.Completed, updated.Status);

        await AssertPersistedAsync(updated);
    }

    [Fact]
    public async Task Test_Update_MissingEntity_ReturnsFail()
    {
        var missingId = OrderId.From(Guid.NewGuid());

        var actual = await _updateService.HandleAsync(new Integration.Requests.UpdateOrderRequest(
            missingId.Value, "ORD-2", null, EStatus.Completed));

        Assert.True(actual.IsFail);
        var message = actual.Match(Succ: _ => string.Empty, Fail: error => error.Message);
        Assert.Equal(PersistenceErrors.NotFound(missingId).Message, message);
    }

    [Fact]
    public async Task Test_Update_OnlyNumber_ChangesOnlyNumber()
    {
        var baseline = await CreateBaselineAsync();

        var updated = await UpdateAsync(new Integration.Requests.UpdateOrderRequest(
            baseline.OrderId, "ORD-CHANGED", null, null));

        Assert.Equal(baseline with { OrderNumber = "ORD-CHANGED" }, updated);
        await AssertPersistedAsync(updated);
    }

    [Fact]
    public async Task Test_Update_OnlyEffectiveDate_ChangesOnlyEffectiveDate()
    {
        var baseline = await CreateBaselineAsync();
        var newEffectiveDate = new DateOnly(2026, 7, 11);

        var updated = await UpdateAsync(new Integration.Requests.UpdateOrderRequest(
            baseline.OrderId, null, newEffectiveDate, null));

        Assert.Equal(baseline with { EffectiveDate = newEffectiveDate }, updated);
        await AssertPersistedAsync(updated);
    }

    [Fact]
    public async Task Test_Update_OnlyStatus_ChangesOnlyStatus()
    {
        var baseline = await CreateBaselineAsync();

        var updated = await UpdateAsync(new Integration.Requests.UpdateOrderRequest(
            baseline.OrderId, null, null, EStatus.Completed));

        Assert.Equal(baseline with { Status = EStatus.Completed }, updated);
        await AssertPersistedAsync(updated);
    }

    [Fact]
    public async Task Test_Update_AllNullMutableFields_PreservesEntity()
    {
        var baseline = await CreateBaselineAsync();

        var updated = await UpdateAsync(new Integration.Requests.UpdateOrderRequest(
            baseline.OrderId, null, null, null));

        Assert.Equal(baseline, updated);
        await AssertPersistedAsync(updated);
    }
}
