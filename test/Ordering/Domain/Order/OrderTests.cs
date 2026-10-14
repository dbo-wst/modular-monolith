using LanguageExt.UnsafeValueAccess;
using Template.Core.Ordering.Domain.Order;
using Template.Core.Ordering.Domain.Order.Events;
using Template.Core.Shared.Kernel.Domain;

namespace Template.Core.Ordering.Domain.Test.Order;

public class OrderAggregateTests
{
    private static readonly OrderNumber DefaultOrderNumber = OrderNumber.From("ORD-1")!;
    private static readonly EffectiveDate DefaultEffectiveDate = EffectiveDate.From(new DateOnly(2026, 7, 10));

    private static OrderAggregate Create() =>
        OrderAggregate.Create(DefaultOrderNumber, DefaultEffectiveDate)
            .ToOption().ValueUnsafe()!;

    private static OrderAggregate Baseline() =>
        OrderAggregate.Rehydrate(
            OrderId.From(Guid.NewGuid()),
            DefaultOrderNumber,
            DefaultEffectiveDate,
            Status.From(EStatus.New));

    // --- Create ---------------------------------------------------------------

    [Fact]
    public void Create_SetsTheFieldsAndStatusNew()
    {
        var result = OrderAggregate.Create(DefaultOrderNumber, DefaultEffectiveDate);

        Assert.True(result.IsSucc);
        var aggregate = result.ToOption().ValueUnsafe();
        Assert.NotNull(aggregate);
        Assert.Equal(DefaultOrderNumber, aggregate.OrderNumber);
        Assert.Equal(DefaultEffectiveDate, aggregate.EffectiveDate);
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
        Assert.Equal(DefaultOrderNumber, created.OrderNumber);
        Assert.Equal(DefaultEffectiveDate, created.EffectiveDate);
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
    public void ChangeDetails_OnlyOrderNumber_ChangesOrderNumber_PreservesRest()
    {
        var aggregate = Baseline();
        var newNumber = OrderNumber.From("ORD-2")!;

        var result = aggregate.ChangeDetails(number: newNumber);

        Assert.True(result.IsSucc);
        Assert.Equal(newNumber, aggregate.OrderNumber);
        AssertUnchangedExcept(aggregate, orderNumber: true);
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
    public void ChangeDetails_OnlyStatus_ChangesStatus_PreservesRest()
    {
        var aggregate = Baseline();

        aggregate.ChangeDetails(status: Status.From(EStatus.Completed));

        Assert.Equal(EStatus.Completed, aggregate.Status!.Value);
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
        var newEffectiveDate = EffectiveDate.From(new DateOnly(2026, 7, 11));

        aggregate.ChangeDetails(null, newEffectiveDate, Status.From(EStatus.Completed));

        var raised = Assert.Single(aggregate.DomainEvents);
        var updated = Assert.IsType<OrderUpdated>(raised);
        Assert.Equal(aggregate.Id, updated.OrderId);
        Assert.Equal(newEffectiveDate, updated.EffectiveDate);
        Assert.Equal(EStatus.Completed, updated.Status!.Value);
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
            orderId, DefaultOrderNumber, DefaultEffectiveDate, Status.From(EStatus.New));

        var snapshot = aggregate.ToSnapshot();

        Assert.Equal(orderId, snapshot.OrderId);
        Assert.Equal(DefaultOrderNumber, snapshot.OrderNumber);
        Assert.Equal(DefaultEffectiveDate, snapshot.EffectiveDate);
        Assert.Equal(EStatus.New, snapshot.Status!.Value);
    }

    [Fact]
    public void FromSnapshot_ThenToSnapshot_RoundTrips()
    {
        var orderId = OrderId.From(Guid.NewGuid());
        var original = new Domain.Order.Order(
            orderId, DefaultOrderNumber, DefaultEffectiveDate, Status.From(EStatus.Completed));

        var roundTripped = OrderAggregate.FromSnapshot(original).ToSnapshot();

        Assert.Equal(original, roundTripped);
    }

    // Asserts every field except those flagged still holds its Baseline() value.
    private static void AssertUnchangedExcept(
        OrderAggregate aggregate,
        bool orderNumber = false,
        bool effectiveDate = false,
        bool status = false)
    {
        if (!orderNumber) Assert.Equal(DefaultOrderNumber, aggregate.OrderNumber);
        if (!effectiveDate) Assert.Equal(DefaultEffectiveDate, aggregate.EffectiveDate);
        if (!status) Assert.Equal(EStatus.New, aggregate.Status!.Value);
    }
}
