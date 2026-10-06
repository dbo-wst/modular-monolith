using LanguageExt.UnsafeValueAccess;
using Wst.Trios.Core.BrokerSide.Domain.Order;
using Wst.Trios.Core.BrokerSide.Persistence;
using Wst.Trios.Core.BrokerSide.TestCompanion;
using CreateOrderRequest = Wst.Trios.Core.BrokerSide.Application.Order.Create;
using ESide = Wst.Trios.Core.BrokerSide.Integration.Dto.ESide;
using EStatus = Wst.Trios.Core.BrokerSide.Integration.Dto.EStatus;
using GetOrderRequest = Wst.Trios.Core.BrokerSide.Application.Order.Get;
using UpdateOrderRequest = Wst.Trios.Core.BrokerSide.Application.Order.Update;

namespace Wst.Trios.Core.BrokerSide.Application.Test.Order.Update;

public class ServiceTests(InMemoryFixture fixture) : IClassFixture<InMemoryFixture>
{
    private readonly CreateOrderRequest.Service _createService = new(new Store(fixture.ContextFactory));
    private readonly UpdateOrderRequest.Service _updateService = new(new Store(fixture.ContextFactory));
    private readonly GetOrderRequest.Service _getService = new(new Store(fixture.ContextFactory));

    private const string BaselineNumber = "ORD-1";
    private static readonly DateOnly BaselineEffectiveDate = new(2026, 7, 10);
    private const decimal BaselineQuantity = 100.5m;
    private const decimal BaselinePrice = 25.75m;
    private const ESide BaselineSide = ESide.Buy;
    private const EStatus BaselineStatus = EStatus.New;

    // --- Success path ---------------------------------------------------------

    [Fact]
    public async Task Test_Update_Get_Success()
    {
        var (brokerId, instrumentId, venueId, order) = await CreateBaselineAsync();

        var newVenueId = Guid.NewGuid();
        var newEffectiveDate = new DateOnly(2026, 7, 11);
        var maybeUpdated = await _updateService.HandleAsync(new Integration.Requests.UpdateOrderRequest(
            order.OrderId, brokerId, instrumentId, newVenueId, BaselineNumber, newEffectiveDate, 200m, 50m, ESide.Sell, EStatus.Filled));

        Assert.True(maybeUpdated.IsSucc);
        var updated = maybeUpdated.ToOption().ValueUnsafe();
        Assert.NotNull(updated);
        Assert.Equal(order.OrderId, updated.Order.OrderId);
        Assert.Equal(newVenueId, updated.Order.VenueId);
        Assert.Equal(newEffectiveDate, updated.Order.EffectiveDate);
        Assert.Equal(200m, updated.Order.Quantity);
        Assert.Equal(50m, updated.Order.Price);
        Assert.Equal(ESide.Sell, updated.Order.Side);
        Assert.Equal(EStatus.Filled, updated.Order.Status);

        // Persisted (round-trips through a fresh Get)
        var maybeOrder = await _getService.HandleAsync(new Integration.Requests.GetOrderRequest(order.OrderId));
        Assert.True(maybeOrder.IsSucc);
        var actual = maybeOrder.ToOption().ValueUnsafe();
        Assert.NotNull(actual);
        Assert.Equal(updated.Order, actual.Order);
    }

    // --- Error case -----------------------------------------------------------

    [Fact]
    public async Task Test_Update_MissingEntity_ReturnsFail()
    {
        var missingId = OrderId.From(Guid.NewGuid());

        var actual = await _updateService.HandleAsync(new Integration.Requests.UpdateOrderRequest(
            missingId.Value, null, null, Guid.NewGuid(), null, null, 200m, 50m, ESide.Buy, EStatus.Filled));

        Assert.True(actual.IsFail);
        var message = actual.Match(Succ: _ => string.Empty, Fail: error => error.Message);
        Assert.Equal(PersistenceErrors.NotFound(missingId).Message, message);
    }

    // --- Single-field updates -------------------------------------------------
    // "null = not updated": every request field except OrderId is nullable, and a
    // null field is left untouched. Each test below sets exactly one field and
    // asserts nothing else moved from its baseline.

    [Fact]
    public async Task Test_Update_OnlyBrokerId_ChangesBrokerId_PreservesRest()
    {
        var (_, instrumentId, venueId, order) = await CreateBaselineAsync();
        var newBrokerId = Guid.NewGuid();

        var updated = await UpdateAsync(new Integration.Requests.UpdateOrderRequest(
            order.OrderId, newBrokerId, null, null, null, null, null, null, null, null));

        Assert.Equal(newBrokerId, updated.BrokerId);
        AssertUnchangedExcept(updated, order.OrderId, instrumentId, venueId, brokerIdShouldBe: newBrokerId);
    }

    [Fact]
    public async Task Test_Update_OnlyInstrumentId_ChangesInstrumentId_PreservesRest()
    {
        var (brokerId, _, venueId, order) = await CreateBaselineAsync();
        var newInstrumentId = Guid.NewGuid();

        var updated = await UpdateAsync(new Integration.Requests.UpdateOrderRequest(
            order.OrderId, null, newInstrumentId, null, null, null, null, null, null, null));

        Assert.Equal(newInstrumentId, updated.InstrumentId);
        AssertUnchangedExcept(updated, order.OrderId, newInstrumentId, venueId, brokerIdShouldBe: brokerId);
    }

    [Fact]
    public async Task Test_Update_OnlyVenueId_ChangesVenueId_PreservesRest()
    {
        var (brokerId, instrumentId, _, order) = await CreateBaselineAsync();
        var newVenueId = Guid.NewGuid();

        var updated = await UpdateAsync(new Integration.Requests.UpdateOrderRequest(
            order.OrderId, null, null, newVenueId, null, null, null, null, null, null));

        Assert.Equal(newVenueId, updated.VenueId);
        AssertUnchangedExcept(updated, order.OrderId, instrumentId, newVenueId, brokerIdShouldBe: brokerId);
    }

    [Fact]
    public async Task Test_Update_OnlyNumber_ChangesNumber_PreservesRest()
    {
        var (brokerId, instrumentId, venueId, order) = await CreateBaselineAsync();

        var updated = await UpdateAsync(new Integration.Requests.UpdateOrderRequest(
            order.OrderId, null, null, null, "ORD-2", null, null, null, null, null));

        Assert.Equal("ORD-2", updated.OrderNumber);
        AssertUnchangedExcept(updated, order.OrderId, instrumentId, venueId, brokerIdShouldBe: brokerId, numberShouldBe: "ORD-2");
    }

    [Fact]
    public async Task Test_Update_OnlyQuantity_ChangesQuantity_PreservesRest()
    {
        var (brokerId, instrumentId, venueId, order) = await CreateBaselineAsync();

        var updated = await UpdateAsync(new Integration.Requests.UpdateOrderRequest(
            order.OrderId, null, null, null, null, null, 200m, null, null, null));

        Assert.Equal(200m, updated.Quantity);
        AssertUnchangedExcept(updated, order.OrderId, instrumentId, venueId, brokerIdShouldBe: brokerId, QuantityShouldBe: 200m);
    }

    [Fact]
    public async Task Test_Update_OnlyEffectiveDate_ChangesEffectiveDate_PreservesRest()
    {
        var (brokerId, instrumentId, venueId, order) = await CreateBaselineAsync();
        var newEffectiveDate = new DateOnly(2026, 7, 11);

        var updated = await UpdateAsync(new Integration.Requests.UpdateOrderRequest(
            order.OrderId, null, null, null, null, newEffectiveDate, null, null, null, null));

        Assert.Equal(newEffectiveDate, updated.EffectiveDate);
        AssertUnchangedExcept(updated, order.OrderId, instrumentId, venueId,
            brokerIdShouldBe: brokerId, effectiveDateShouldBe: newEffectiveDate);
    }

    [Fact]
    public async Task Test_Update_OnlyPrice_ChangesPrice_PreservesRest()
    {
        var (brokerId, instrumentId, venueId, order) = await CreateBaselineAsync();

        var updated = await UpdateAsync(new Integration.Requests.UpdateOrderRequest(
            order.OrderId, null, null, null, null, null, null, 99.99m, null, null));

        Assert.Equal(99.99m, updated.Price);
        AssertUnchangedExcept(updated, order.OrderId, instrumentId, venueId, brokerIdShouldBe: brokerId, priceShouldBe: 99.99m);
    }

    [Fact]
    public async Task Test_Update_OnlySide_ChangesSide_PreservesRest()
    {
        var (brokerId, instrumentId, venueId, order) = await CreateBaselineAsync();

        var updated = await UpdateAsync(new Integration.Requests.UpdateOrderRequest(
            order.OrderId, null, null, null, null, null, null, null, ESide.Sell, null));

        Assert.Equal(ESide.Sell, updated.Side);
        AssertUnchangedExcept(updated, order.OrderId, instrumentId, venueId, brokerIdShouldBe: brokerId, sideShouldBe: ESide.Sell);
    }

    [Fact]
    public async Task Test_Update_OnlyStatus_ChangesStatus_PreservesRest()
    {
        var (brokerId, instrumentId, venueId, order) = await CreateBaselineAsync();

        var updated = await UpdateAsync(new Integration.Requests.UpdateOrderRequest(
            order.OrderId, null, null, null, null, null, null, null, null, EStatus.Filled));

        Assert.Equal(EStatus.Filled, updated.Status);
        AssertUnchangedExcept(updated, order.OrderId, instrumentId, venueId, brokerIdShouldBe: brokerId, statusShouldBe: EStatus.Filled);
    }

    [Fact]
    public async Task Test_Update_AllNullExceptId_IsNoOp()
    {
        var (brokerId, instrumentId, venueId, order) = await CreateBaselineAsync();

        var updated = await UpdateAsync(new Integration.Requests.UpdateOrderRequest(
            order.OrderId, null, null, null, null, null, null, null, null, null));

        AssertUnchangedExcept(updated, order.OrderId, instrumentId, venueId, brokerIdShouldBe: brokerId);
        Assert.Equal(order, updated);
    }

    // --- Helpers --------------------------------------------------------------

    private async Task<(Guid BrokerId, Guid InstrumentId, Guid VenueId, Integration.Dto.Order Order)> CreateBaselineAsync()
    {
        var brokerId = Guid.NewGuid();
        var instrumentId = Guid.NewGuid();
        var venueId = Guid.NewGuid();

        var maybeCreated = await _createService.HandleAsync(new Integration.Requests.CreateOrderRequest(
            brokerId, instrumentId, venueId, BaselineNumber, BaselineEffectiveDate, BaselineQuantity, BaselinePrice, BaselineSide));

        Assert.True(maybeCreated.IsSucc);
        var created = maybeCreated.ToOption().ValueUnsafe();
        Assert.NotNull(created);
        return (brokerId, instrumentId, venueId, created.Order);
    }

    private async Task<Integration.Dto.Order> UpdateAsync(Integration.Requests.UpdateOrderRequest request)
    {
        var maybeUpdated = await _updateService.HandleAsync(request);
        Assert.True(maybeUpdated.IsSucc);
        var updated = maybeUpdated.ToOption().ValueUnsafe();
        Assert.NotNull(updated);
        return updated.Order;
    }

    // Asserts every field except the one(s) explicitly overridden still holds baseline.
    private static void AssertUnchangedExcept(
        Integration.Dto.Order updated,
        Guid orderId,
        Guid instrumentId,
        Guid venueId,
        Guid? brokerIdShouldBe = null,
        string? numberShouldBe = null,
        DateOnly? effectiveDateShouldBe = null,
        decimal? QuantityShouldBe = null,
        decimal? priceShouldBe = null,
        ESide? sideShouldBe = BaselineSide,
        EStatus? statusShouldBe = BaselineStatus)
    {
        Assert.Equal(orderId, updated.OrderId);
        Assert.Equal(brokerIdShouldBe, updated.BrokerId);
        Assert.Equal(instrumentId, updated.InstrumentId);
        Assert.Equal(venueId, updated.VenueId);
        Assert.Equal(numberShouldBe ?? BaselineNumber, updated.OrderNumber);
        Assert.Equal(effectiveDateShouldBe ?? BaselineEffectiveDate, updated.EffectiveDate);
        Assert.Equal(QuantityShouldBe ?? BaselineQuantity, updated.Quantity);
        Assert.Equal(priceShouldBe ?? BaselinePrice, updated.Price);
        Assert.Equal(sideShouldBe, updated.Side);
        Assert.Equal(statusShouldBe, updated.Status);
    }
}
