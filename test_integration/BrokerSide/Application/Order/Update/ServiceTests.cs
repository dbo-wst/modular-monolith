using LanguageExt.UnsafeValueAccess;
using Wst.Trios.Core.BrokerSide.Persistence;
using Wst.Trios.Core.BrokerSide.TestCompanion;
using CreateOrderRequest = Wst.Trios.Core.BrokerSide.Application.Order.Create;
using Dto = Wst.Trios.Core.BrokerSide.Integration.Dto;
using ESide = Wst.Trios.Core.BrokerSide.Integration.Dto.ESide;
using EStatus = Wst.Trios.Core.BrokerSide.Integration.Dto.EStatus;
using GetOrderRequest = Wst.Trios.Core.BrokerSide.Application.Order.Get;
using OrderId = Wst.Trios.Core.BrokerSide.Domain.Order.OrderId;
using UpdateOrderRequest = Wst.Trios.Core.BrokerSide.Application.Order.Update;

namespace Wst.Trios.Core.BrokerSide.Application.IntegrationTest.Order.Update;

[Collection(IntegrationTestsFixture.CollectionName)]
public class ServiceTests(IntegrationTestsFixture fixture)
{
    private readonly CreateOrderRequest.Service _createService = new(new Store(fixture.GetRequiredService<IBrokerSideDbContextFactory>()));
    private readonly UpdateOrderRequest.Service _updateService = new(new Store(fixture.GetRequiredService<IBrokerSideDbContextFactory>()));
    private readonly GetOrderRequest.Service _getService = new(new Store(fixture.GetRequiredService<IBrokerSideDbContextFactory>()));

    private async Task<Dto.Order> CreateBaselineAsync()
    {
        var maybeCreated = await _createService.HandleAsync(new Integration.Requests.CreateOrderRequest(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "ORD-1", new DateOnly(2026, 7, 10), 100.5m, 25.75m, ESide.Buy));
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

        var newBrokerId = Guid.NewGuid();
        var newInstrumentId = Guid.NewGuid();
        var newVenueId = Guid.NewGuid();
        var newEffectiveDate = new DateOnly(2026, 7, 11);
        var updated = await UpdateAsync(new Integration.Requests.UpdateOrderRequest(
            baseline.OrderId, newBrokerId, newInstrumentId, newVenueId, "ORD-2", newEffectiveDate, 200m, 50m, ESide.Sell, EStatus.Filled));

        Assert.Equal(baseline.OrderId, updated.OrderId);
        Assert.Equal(newBrokerId, updated.BrokerId);
        Assert.Equal(newInstrumentId, updated.InstrumentId);
        Assert.Equal(newVenueId, updated.VenueId);
        Assert.Equal("ORD-2", updated.OrderNumber);
        Assert.Equal(newEffectiveDate, updated.EffectiveDate);
        Assert.Equal(200m, updated.Quantity);
        Assert.Equal(50m, updated.Price);
        Assert.Equal(ESide.Sell, updated.Side);
        Assert.Equal(EStatus.Filled, updated.Status);

        await AssertPersistedAsync(updated);
    }

    [Fact]
    public async Task Test_Update_MissingEntity_ReturnsFail()
    {
        var missingId = OrderId.From(Guid.NewGuid());

        var actual = await _updateService.HandleAsync(new Integration.Requests.UpdateOrderRequest(
            missingId.Value, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "ORD-2", null, 200m, 50m, ESide.Sell, EStatus.Filled));

        Assert.True(actual.IsFail);
        var message = actual.Match(Succ: _ => string.Empty, Fail: error => error.Message);
        Assert.Equal(PersistenceErrors.NotFound(missingId).Message, message);
    }

    [Fact]
    public async Task Test_Update_OnlyBrokerId_ChangesOnlyBrokerId()
    {
        var baseline = await CreateBaselineAsync();
        var newBrokerId = Guid.NewGuid();

        var updated = await UpdateAsync(new Integration.Requests.UpdateOrderRequest(
            baseline.OrderId, newBrokerId, null, null, null, null, null, null, null, null));

        Assert.Equal(baseline with { BrokerId = newBrokerId }, updated);
        await AssertPersistedAsync(updated);
    }

    [Fact]
    public async Task Test_Update_OnlyInstrumentId_ChangesOnlyInstrumentId()
    {
        var baseline = await CreateBaselineAsync();
        var newInstrumentId = Guid.NewGuid();

        var updated = await UpdateAsync(new Integration.Requests.UpdateOrderRequest(
            baseline.OrderId, null, newInstrumentId, null, null, null, null, null, null, null));

        Assert.Equal(baseline with { InstrumentId = newInstrumentId }, updated);
        await AssertPersistedAsync(updated);
    }

    [Fact]
    public async Task Test_Update_OnlyVenueId_ChangesOnlyVenueId()
    {
        var baseline = await CreateBaselineAsync();
        var newVenueId = Guid.NewGuid();

        var updated = await UpdateAsync(new Integration.Requests.UpdateOrderRequest(
            baseline.OrderId, null, null, newVenueId, null, null, null, null, null, null));

        Assert.Equal(baseline with { VenueId = newVenueId }, updated);
        await AssertPersistedAsync(updated);
    }

    [Fact]
    public async Task Test_Update_OnlyNumber_ChangesOnlyNumber()
    {
        var baseline = await CreateBaselineAsync();

        var updated = await UpdateAsync(new Integration.Requests.UpdateOrderRequest(
            baseline.OrderId, null, null, null, "ORD-CHANGED", null, null, null, null, null));

        Assert.Equal(baseline with { OrderNumber = "ORD-CHANGED" }, updated);
        await AssertPersistedAsync(updated);
    }

    [Fact]
    public async Task Test_Update_OnlyQuantity_ChangesOnlyQuantity()
    {
        var baseline = await CreateBaselineAsync();

        var updated = await UpdateAsync(new Integration.Requests.UpdateOrderRequest(
            baseline.OrderId, null, null, null, null, null, 999m, null, null, null));

        Assert.Equal(baseline with { Quantity = 999m }, updated);
        await AssertPersistedAsync(updated);
    }

    [Fact]
    public async Task Test_Update_OnlyEffectiveDate_ChangesOnlyEffectiveDate()
    {
        var baseline = await CreateBaselineAsync();
        var newEffectiveDate = new DateOnly(2026, 7, 11);

        var updated = await UpdateAsync(new Integration.Requests.UpdateOrderRequest(
            baseline.OrderId, null, null, null, null, newEffectiveDate, null, null, null, null));

        Assert.Equal(baseline with { EffectiveDate = newEffectiveDate }, updated);
        await AssertPersistedAsync(updated);
    }

    [Fact]
    public async Task Test_Update_OnlyPrice_ChangesOnlyPrice()
    {
        var baseline = await CreateBaselineAsync();

        var updated = await UpdateAsync(new Integration.Requests.UpdateOrderRequest(
            baseline.OrderId, null, null, null, null, null, null, 12.34m, null, null));

        Assert.Equal(baseline with { Price = 12.34m }, updated);
        await AssertPersistedAsync(updated);
    }

    [Fact]
    public async Task Test_Update_OnlySide_ChangesOnlySide()
    {
        var baseline = await CreateBaselineAsync();

        var updated = await UpdateAsync(new Integration.Requests.UpdateOrderRequest(
            baseline.OrderId, null, null, null, null, null, null, null, ESide.Sell, null));

        Assert.Equal(baseline with { Side = ESide.Sell }, updated);
        await AssertPersistedAsync(updated);
    }

    [Fact]
    public async Task Test_Update_OnlyStatus_ChangesOnlyStatus()
    {
        var baseline = await CreateBaselineAsync();

        var updated = await UpdateAsync(new Integration.Requests.UpdateOrderRequest(
            baseline.OrderId, null, null, null, null, null, null, null, null, EStatus.Filled));

        Assert.Equal(baseline with { Status = EStatus.Filled }, updated);
        await AssertPersistedAsync(updated);
    }

    [Fact]
    public async Task Test_Update_AllNullMutableFields_PreservesEntity()
    {
        var baseline = await CreateBaselineAsync();

        var updated = await UpdateAsync(new Integration.Requests.UpdateOrderRequest(
            baseline.OrderId, null, null, null, null, null, null, null, null, null));

        Assert.Equal(baseline, updated);
        await AssertPersistedAsync(updated);
    }
}
