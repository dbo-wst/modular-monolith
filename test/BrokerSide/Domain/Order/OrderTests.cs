using LanguageExt.UnsafeValueAccess;
using Wst.Trios.Core.BrokerSide.Domain.Order;
using Wst.Trios.Core.BrokerSide.Domain.Order.Events;
using Wst.Trios.Core.Shared.Kernel.Domain;

namespace Wst.Trios.Core.BrokerSide.Domain.Test.Order;

public class OrderAggregateTests
{
    private static readonly BrokerId DefaultBrokerId = BrokerId.From(Guid.NewGuid());
    private static readonly InstrumentId DefaultInstrumentId = InstrumentId.From(Guid.NewGuid());
    private static readonly VenueId DefaultVenueId = VenueId.From(Guid.NewGuid());
    private static readonly OrderNumber DefaultOrderNumber = OrderNumber.From("ORD-1")!;
    private static readonly EffectiveDate DefaultEffectiveDate = EffectiveDate.From(new DateOnly(2026, 7, 10));

    private static OrderAggregate Create(ESide? side = ESide.Buy) =>
        OrderAggregate.Create(
                DefaultBrokerId,
                DefaultInstrumentId,
                DefaultVenueId,
                DefaultOrderNumber,
                DefaultEffectiveDate,
                Quantity.From(100.5m),
                Price.From(25.75m),
                Side.From(side))
            .ToOption().ValueUnsafe()!;

    private static OrderAggregate Baseline() =>
        OrderAggregate.Rehydrate(
            OrderId.From(Guid.NewGuid()),
            DefaultBrokerId,
            DefaultInstrumentId,
            DefaultVenueId,
            DefaultOrderNumber,
            DefaultEffectiveDate,
            Quantity.From(100.5m),
            Price.From(25.75m),
            Side.From(ESide.Buy),
            Status.From(EStatus.New));

    // --- Create ---------------------------------------------------------------

    [Fact]
    public void Create_SetsTheFieldsAndStatusNew()
    {
        var result = OrderAggregate.Create(
            DefaultBrokerId, DefaultInstrumentId, DefaultVenueId, DefaultOrderNumber,
            DefaultEffectiveDate, Quantity.From(100.5m), Price.From(25.75m), Side.From(ESide.Buy));

        Assert.True(result.IsSucc);
        var aggregate = result.ToOption().ValueUnsafe();
        Assert.NotNull(aggregate);
        Assert.Equal(DefaultBrokerId, aggregate.BrokerId);
        Assert.Equal(DefaultInstrumentId, aggregate.InstrumentId);
        Assert.Equal(DefaultVenueId, aggregate.VenueId);
        Assert.Equal(DefaultOrderNumber, aggregate.OrderNumber);
        Assert.Equal(DefaultEffectiveDate, aggregate.EffectiveDate);
        Assert.Equal(100.5m, aggregate.Quantity!.Value);
        Assert.Equal(25.75m, aggregate.Price!.Value);
        Assert.Equal(ESide.Buy, aggregate.Side!.Value);
        Assert.Equal(EStatus.New, aggregate.Status!.Value);
    }

    [Fact]
    public void Create_AssignsANonEmptyOrderId()
    {
        var aggregate = Create();

        Assert.NotEqual(Guid.Empty, aggregate.Id.Value);
    }

    [Fact]
    public void Create_RaisesOrderCreated()
    {
        var aggregate = Create();

        var raised = Assert.Single(aggregate.DomainEvents);
        var created = Assert.IsType<OrderCreated>(raised);
        Assert.Equal(aggregate.Id, created.OrderId);
        Assert.Equal(DefaultBrokerId, created.BrokerId);
        Assert.Equal(DefaultInstrumentId, created.InstrumentId);
        Assert.Equal(DefaultVenueId, created.VenueId);
        Assert.Equal(DefaultOrderNumber, created.OrderNumber);
        Assert.Equal(DefaultEffectiveDate, created.EffectiveDate);
        Assert.Equal(100.5m, created.Quantity!.Value);
        Assert.Equal(25.75m, created.Price!.Value);
        Assert.Equal(ESide.Buy, created.Side!.Value);
    }

    [Fact]
    public void Create_WithNullSide_AllowsNullSide()
    {
        var aggregate = Create(side: null);

        Assert.Null(aggregate.Side);
        Assert.Null(aggregate.ToSnapshot().Side);
    }

    [Fact]
    public void Create_GeneratesADistinctOrderIdPerCall()
    {
        var first = Create();
        var second = Create();

        Assert.NotEqual(first.Id, second.Id);
    }

    // --- ChangeDetails: null = not updated (single-field) ---------------------

    [Fact]
    public void ChangeDetails_OnlyBrokerId_ChangesBrokerId_PreservesRest()
    {
        var aggregate = Baseline();
        var newBrokerId = BrokerId.From(Guid.NewGuid());

        var result = aggregate.ChangeDetails(brokerId: newBrokerId);

        Assert.True(result.IsSucc);
        Assert.Equal(newBrokerId, aggregate.BrokerId);
        AssertUnchangedExcept(aggregate, brokerId: true);
    }

    [Fact]
    public void ChangeDetails_OnlyInstrumentId_ChangesInstrumentId_PreservesRest()
    {
        var aggregate = Baseline();
        var newInstrumentId = InstrumentId.From(Guid.NewGuid());

        aggregate.ChangeDetails(instrumentId: newInstrumentId);

        Assert.Equal(newInstrumentId, aggregate.InstrumentId);
        AssertUnchangedExcept(aggregate, instrumentId: true);
    }

    [Fact]
    public void ChangeDetails_OnlyVenueId_ChangesVenueId_PreservesRest()
    {
        var aggregate = Baseline();
        var newVenueId = VenueId.From(Guid.NewGuid());

        aggregate.ChangeDetails(venueId: newVenueId);

        Assert.Equal(newVenueId, aggregate.VenueId);
        AssertUnchangedExcept(aggregate, venueId: true);
    }

    [Fact]
    public void ChangeDetails_OnlyOrderNumber_ChangesOrderNumber_PreservesRest()
    {
        var aggregate = Baseline();
        var newNumber = OrderNumber.From("ORD-2")!;

        aggregate.ChangeDetails(number: newNumber);

        Assert.Equal(newNumber, aggregate.OrderNumber);
        AssertUnchangedExcept(aggregate, orderNumber: true);
    }

    [Fact]
    public void ChangeDetails_OnlyQuantity_ChangesQuantity_PreservesRest()
    {
        var aggregate = Baseline();

        aggregate.ChangeDetails(quantity: Quantity.From(200m));

        Assert.Equal(200m, aggregate.Quantity!.Value);
        AssertUnchangedExcept(aggregate, quantity: true);
    }

    [Fact]
    public void ChangeDetails_OnlyEffectiveDate_ChangesEffectiveDate_PreservesRest()
    {
        var aggregate = Baseline();
        var newEffectiveDate = EffectiveDate.From(new DateOnly(2026, 7, 11));

        aggregate.ChangeDetails(effectiveDate: newEffectiveDate);

        Assert.Equal(newEffectiveDate, aggregate.EffectiveDate);
        AssertUnchangedExcept(aggregate, effectiveDate: true);
    }

    [Fact]
    public void ChangeDetails_OnlyPrice_ChangesPrice_PreservesRest()
    {
        var aggregate = Baseline();

        aggregate.ChangeDetails(price: Price.From(99.99m));

        Assert.Equal(99.99m, aggregate.Price!.Value);
        AssertUnchangedExcept(aggregate, price: true);
    }

    [Fact]
    public void ChangeDetails_OnlySide_ChangesSide_PreservesRest()
    {
        var aggregate = Baseline();

        aggregate.ChangeDetails(side: Side.From(ESide.Sell));

        Assert.Equal(ESide.Sell, aggregate.Side!.Value);
        AssertUnchangedExcept(aggregate, side: true);
    }

    [Fact]
    public void ChangeDetails_OnlyStatus_ChangesStatus_PreservesRest()
    {
        var aggregate = Baseline();

        aggregate.ChangeDetails(status: Status.From(EStatus.Filled));

        Assert.Equal(EStatus.Filled, aggregate.Status!.Value);
        AssertUnchangedExcept(aggregate, status: true);
    }

    [Fact]
    public void ChangeDetails_AllNull_ChangesNothing()
    {
        var aggregate = Baseline();

        var result = aggregate.ChangeDetails();

        Assert.True(result.IsSucc);
        AssertUnchangedExcept(aggregate);
    }

    [Fact]
    public void ChangeDetails_RaisesOrderUpdated()
    {
        var aggregate = Baseline();
        var newVenueId = VenueId.From(Guid.NewGuid());
        var newEffectiveDate = EffectiveDate.From(new DateOnly(2026, 7, 11));

        aggregate.ChangeDetails(null, null, newVenueId, null, newEffectiveDate, Quantity.From(200m), Price.From(50m), Side.From(ESide.Sell), Status.From(EStatus.Filled));

        var raised = Assert.Single(aggregate.DomainEvents);
        var updated = Assert.IsType<OrderUpdated>(raised);
        Assert.Equal(aggregate.Id, updated.OrderId);
        Assert.Equal(newVenueId, updated.VenueId);
        Assert.Equal(newEffectiveDate, updated.EffectiveDate);
        Assert.Equal(200m, updated.Quantity!.Value);
        Assert.Equal(50m, updated.Price!.Value);
        Assert.Equal(ESide.Sell, updated.Side!.Value);
        Assert.Equal(EStatus.Filled, updated.Status!.Value);
    }

    // --- Delete ---------------------------------------------------------------

    [Fact]
    public void Delete_RaisesOrderDeleted()
    {
        var aggregate = Baseline();

        var result = aggregate.Delete();

        Assert.True(result.IsSucc);
        var raised = Assert.Single(aggregate.DomainEvents);
        var deleted = Assert.IsType<OrderDeleted>(raised);
        Assert.Equal(aggregate.Id, deleted.OrderId);
    }

    // --- Snapshot round-trip --------------------------------------------------

    [Fact]
    public void ToSnapshot_ProjectsAllFields()
    {
        var orderId = OrderId.From(Guid.NewGuid());
        var aggregate = OrderAggregate.Rehydrate(
            orderId, DefaultBrokerId, DefaultInstrumentId, DefaultVenueId, DefaultOrderNumber,
            DefaultEffectiveDate, Quantity.From(100.5m), Price.From(25.75m), Side.From(ESide.Buy), Status.From(EStatus.New));

        var snapshot = aggregate.ToSnapshot();

        Assert.Equal(orderId, snapshot.OrderId);
        Assert.Equal(DefaultBrokerId, snapshot.BrokerId);
        Assert.Equal(DefaultInstrumentId, snapshot.InstrumentId);
        Assert.Equal(DefaultVenueId, snapshot.VenueId);
        Assert.Equal(DefaultOrderNumber, snapshot.OrderNumber);
        Assert.Equal(DefaultEffectiveDate, snapshot.EffectiveDate);
        Assert.Equal(100.5m, snapshot.Quantity!.Value);
        Assert.Equal(25.75m, snapshot.Price!.Value);
        Assert.Equal(ESide.Buy, snapshot.Side!.Value);
        Assert.Equal(EStatus.New, snapshot.Status!.Value);
    }

    [Fact]
    public void FromSnapshot_ThenToSnapshot_RoundTrips()
    {
        var orderId = OrderId.From(Guid.NewGuid());
        var original = new Domain.Order.Order(
            orderId, DefaultBrokerId, DefaultInstrumentId, DefaultVenueId, DefaultOrderNumber,
            DefaultEffectiveDate, Quantity.From(100.5m), Price.From(25.75m), Side.From(ESide.Buy), Status.From(EStatus.Filled));

        var roundTripped = OrderAggregate.FromSnapshot(original).ToSnapshot();

        Assert.Equal(original, roundTripped);
    }

    // Asserts every field except those flagged still holds its Baseline() value.
    private static void AssertUnchangedExcept(
        OrderAggregate aggregate,
        bool brokerId = false,
        bool instrumentId = false,
        bool venueId = false,
        bool orderNumber = false,
        bool effectiveDate = false,
        bool quantity = false,
        bool price = false,
        bool side = false,
        bool status = false)
    {
        if (!brokerId) Assert.Equal(DefaultBrokerId, aggregate.BrokerId);
        if (!instrumentId) Assert.Equal(DefaultInstrumentId, aggregate.InstrumentId);
        if (!venueId) Assert.Equal(DefaultVenueId, aggregate.VenueId);
        if (!orderNumber) Assert.Equal(DefaultOrderNumber, aggregate.OrderNumber);
        if (!effectiveDate) Assert.Equal(DefaultEffectiveDate, aggregate.EffectiveDate);
        if (!quantity) Assert.Equal(100.5m, aggregate.Quantity!.Value);
        if (!price) Assert.Equal(25.75m, aggregate.Price!.Value);
        if (!side) Assert.Equal(ESide.Buy, aggregate.Side!.Value);
        if (!status) Assert.Equal(EStatus.New, aggregate.Status!.Value);
    }
}
