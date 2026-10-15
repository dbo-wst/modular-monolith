using LanguageExt;
using Acme.Core.BuildingBlocks.Domain.Errors;
using Acme.Core.Ordering.Domain.Orders;
using Acme.Core.Ordering.Domain.Orders.States;
using Acme.Core.Ordering.Domain.ValueObjects;
using DomainOrder = Acme.Core.Ordering.Domain.Orders.Order;

namespace Acme.Core.Ordering.Domain.Test.Order;

public class OrderStateTests
{
    private static DomainOrder InStatus(OrderStatus status) =>
        DomainOrder.Rehydrate(
            OrderId.From(Guid.NewGuid()),
            new OrderNumber("ORD-1"),
            EffectiveDate.From(new DateOnly(2026, 7, 10)),
            status);

    [Theory]
    [InlineData(OrderStatus.New, typeof(NewOrderState))]
    [InlineData(OrderStatus.InProgress, typeof(InProgressOrderState))]
    [InlineData(OrderStatus.Completed, typeof(CompletedOrderState))]
    [InlineData(OrderStatus.Cancelled, typeof(CancelledOrderState))]
    [InlineData(OrderStatus.Rejected, typeof(RejectedOrderState))]
    public void From_EveryStatus_ReturnsStateWithThatStatus(OrderStatus status, Type expectedType)
    {
        var state = IOrderState.From(status);

        Assert.IsType(expectedType, state);
        Assert.Equal(status, state.Status);
    }

    [Fact]
    public void From_UndefinedStatus_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => IOrderState.From((OrderStatus)999));
    }

    [Theory]
    [InlineData(OrderStatus.Completed, "Start")]
    [InlineData(OrderStatus.Completed, "Complete")]
    [InlineData(OrderStatus.Completed, "Cancel")]
    [InlineData(OrderStatus.Completed, "Reject")]
    [InlineData(OrderStatus.Cancelled, "Start")]
    [InlineData(OrderStatus.Cancelled, "Complete")]
    [InlineData(OrderStatus.Cancelled, "Cancel")]
    [InlineData(OrderStatus.Cancelled, "Reject")]
    [InlineData(OrderStatus.Rejected, "Start")]
    [InlineData(OrderStatus.Rejected, "Complete")]
    [InlineData(OrderStatus.Rejected, "Cancel")]
    [InlineData(OrderStatus.Rejected, "Reject")]
    public void TerminalState_AnyTransition_FailsWithValidationError_AndKeepsStatus(OrderStatus terminal, string transition)
    {
        var order = InStatus(terminal);
        var state = IOrderState.From(terminal);

        Fin<Unit> result = transition switch
        {
            "Start" => state.Start(order),
            "Complete" => state.Complete(order),
            "Cancel" => state.Cancel(order),
            "Reject" => state.Reject(order),
            _ => throw new ArgumentOutOfRangeException(nameof(transition))
        };

        Assert.True(result.IsFail);
        result.IfFail(error =>
        {
            var validation = Assert.IsType<ValidationError>(error);
            Assert.Equal(ErrorCodes.Validation, validation.ErrorCode);
        });
        Assert.Equal(terminal, order.Status);
        Assert.Empty(order.DomainEvents);
    }
}
