using LanguageExt;
using LanguageExt.UnsafeValueAccess;
using Template.Core.Ordering.Domain.Orders;
using Template.Core.Ordering.Domain.Orders.Events;
using Template.Core.Ordering.Domain.ValueObjects;
using DomainOrder = Template.Core.Ordering.Domain.Orders.Order;

namespace Template.Core.Ordering.Domain.Test.Order;

public class OrderTests
{
    private static readonly OrderNumber DefaultOrderNumber = new("ORD-1");
    private static readonly EffectiveDate DefaultEffectiveDate = EffectiveDate.From(new DateOnly(2026, 7, 10));

    private static DomainOrder Create() =>
        DomainOrder.Create(DefaultOrderNumber, DefaultEffectiveDate).ToOption().ValueUnsafe()!;

    private static DomainOrder InStatus(OrderStatus status) =>
        DomainOrder.Rehydrate(OrderId.From(Guid.NewGuid()), DefaultOrderNumber, DefaultEffectiveDate, status);

    private static DomainOrder Baseline() => InStatus(OrderStatus.New);

    // --- Create ---------------------------------------------------------------

    [Fact]
    public void Create_SetsTheFieldsAndStatusNew()
    {
        var result = DomainOrder.Create(DefaultOrderNumber, DefaultEffectiveDate);

        Assert.True(result.IsSucc);
        var aggregate = result.ToOption().ValueUnsafe()!;
        Assert.Equal(DefaultOrderNumber, aggregate.OrderNumber);
        Assert.Equal(DefaultEffectiveDate, aggregate.EffectiveDate);
        Assert.Equal(OrderStatus.New, aggregate.Status);
        Assert.NotEqual(Guid.Empty, aggregate.Id.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_BlankNumber_Fails(string number)
    {
        var result = DomainOrder.Create(new OrderNumber(number), DefaultEffectiveDate);

        Assert.True(result.IsFail);
    }

    [Fact]
    public void Create_NullNumberOrDate_Fails()
    {
        Assert.True(DomainOrder.Create(null!, DefaultEffectiveDate).IsFail);
        Assert.True(DomainOrder.Create(DefaultOrderNumber, null!).IsFail);
    }

    [Fact]
    public void Create_RaisesOrderCreatedWithFullState()
    {
        var aggregate = Create();

        var created = Assert.IsType<OrderCreated>(Assert.Single(aggregate.DomainEvents));
        Assert.Equal(aggregate.Id, created.OrderId);
        Assert.Equal(DefaultOrderNumber, created.OrderNumber);
        Assert.Equal(DefaultEffectiveDate, created.EffectiveDate);
    }

    [Fact]
    public void Create_GeneratesADistinctOrderIdPerCall()
    {
        Assert.NotEqual(Create().Id, Create().Id);
    }

    // --- Rehydrate ------------------------------------------------------------

    [Fact]
    public void Rehydrate_IsPermissive_AndRaisesNoEvents()
    {
        var aggregate = DomainOrder.Rehydrate(OrderId.From(Guid.NewGuid()));

        Assert.Equal(OrderStatus.New, aggregate.Status);
        Assert.Empty(aggregate.DomainEvents);
    }

    // --- ChangeDetails --------------------------------------------------------

    [Fact]
    public void ChangeDetails_OnlyOrderNumber_ChangesOrderNumber_PreservesRest()
    {
        var aggregate = Baseline();
        var newNumber = new OrderNumber("ORD-2");

        aggregate.ChangeDetails(number: newNumber);

        Assert.Equal(newNumber, aggregate.OrderNumber);
        Assert.Equal(DefaultEffectiveDate, aggregate.EffectiveDate);
        Assert.Equal(OrderStatus.New, aggregate.Status);
    }

    [Fact]
    public void ChangeDetails_OnlyEffectiveDate_ChangesEffectiveDate_PreservesRest()
    {
        var aggregate = Baseline();
        var newDate = EffectiveDate.From(new DateOnly(2026, 7, 11));

        aggregate.ChangeDetails(effectiveDate: newDate);

        Assert.Equal(newDate, aggregate.EffectiveDate);
        Assert.Equal(DefaultOrderNumber, aggregate.OrderNumber);
    }

    [Fact]
    public void ChangeDetails_RaisesOrderUpdatedWithFullState()
    {
        var aggregate = Baseline();
        var newDate = EffectiveDate.From(new DateOnly(2026, 7, 11));

        aggregate.ChangeDetails(null, newDate);

        var updated = Assert.IsType<OrderUpdated>(Assert.Single(aggregate.DomainEvents));
        Assert.Equal(aggregate.Id, updated.OrderId);
        Assert.Equal(DefaultOrderNumber, updated.Number);
        Assert.Equal(newDate, updated.EffectiveDate);
        Assert.Equal(OrderStatus.New, updated.Status);
    }

    [Fact]
    public void ChangeDetails_NothingProvided_RaisesNoEvent()
    {
        var aggregate = Baseline();

        aggregate.ChangeDetails();

        Assert.Empty(aggregate.DomainEvents);
    }

    [Fact]
    public void ChangeDetails_SameValues_RaisesNoEvent()
    {
        var aggregate = Baseline();

        aggregate.ChangeDetails(new OrderNumber("ORD-1"), EffectiveDate.From(new DateOnly(2026, 7, 10)));

        Assert.Empty(aggregate.DomainEvents);
    }

    [Fact]
    public void ChangeDetails_BlankNumber_IsIgnored()
    {
        var aggregate = Baseline();

        aggregate.ChangeDetails(new OrderNumber(" "));

        Assert.Equal(DefaultOrderNumber, aggregate.OrderNumber);
        Assert.Empty(aggregate.DomainEvents);
    }

    // --- Transitions ----------------------------------------------------------

    [Theory]
    [InlineData(OrderStatus.New, OrderStatus.InProgress, "Start")]
    [InlineData(OrderStatus.InProgress, OrderStatus.Completed, "Complete")]
    [InlineData(OrderStatus.New, OrderStatus.Cancelled, "Cancel")]
    [InlineData(OrderStatus.InProgress, OrderStatus.Cancelled, "Cancel")]
    [InlineData(OrderStatus.New, OrderStatus.Rejected, "Reject")]
    [InlineData(OrderStatus.InProgress, OrderStatus.Rejected, "Reject")]
    public void Transition_Valid_ChangesStatus_AndRaisesOrderUpdated(OrderStatus from, OrderStatus to, string method)
    {
        var aggregate = InStatus(from);

        var result = Apply(aggregate, method);

        Assert.True(result.IsSucc);
        Assert.Equal(to, aggregate.Status);
        var updated = Assert.IsType<OrderUpdated>(Assert.Single(aggregate.DomainEvents));
        Assert.Equal(to, updated.Status);
        Assert.Equal(DefaultOrderNumber, updated.Number);
    }

    [Theory]
    [InlineData(OrderStatus.InProgress, "Start")]
    [InlineData(OrderStatus.Completed, "Start")]
    [InlineData(OrderStatus.New, "Complete")]
    [InlineData(OrderStatus.Completed, "Complete")]
    [InlineData(OrderStatus.Cancelled, "Complete")]
    [InlineData(OrderStatus.Completed, "Cancel")]
    [InlineData(OrderStatus.Cancelled, "Cancel")]
    [InlineData(OrderStatus.Rejected, "Cancel")]
    [InlineData(OrderStatus.Completed, "Reject")]
    [InlineData(OrderStatus.Cancelled, "Reject")]
    [InlineData(OrderStatus.Rejected, "Reject")]
    public void Transition_Invalid_Fails_KeepsStatus_AndRaisesNoEvent(OrderStatus from, string method)
    {
        var aggregate = InStatus(from);

        var result = Apply(aggregate, method);

        Assert.True(result.IsFail);
        Assert.Equal(from, aggregate.Status);
        Assert.Empty(aggregate.DomainEvents);
    }

    private static Fin<DomainOrder> Apply(DomainOrder order, string method) => method switch
    {
        "Start" => order.Start(),
        "Complete" => order.Complete(),
        "Cancel" => order.Cancel(),
        "Reject" => order.Reject(),
        _ => throw new ArgumentOutOfRangeException(nameof(method))
    };

    // --- Delete ---------------------------------------------------------------

    [Fact]
    public void Delete_RaisesOrderDeleted()
    {
        var aggregate = Baseline();

        aggregate.Delete();

        var deleted = Assert.IsType<OrderDeleted>(Assert.Single(aggregate.DomainEvents));
        Assert.Equal(aggregate.Id, deleted.OrderId);
    }
}
