using FluentValidation;
using Acme.Core.Ordering.Contracts.Commands;

namespace Acme.Core.Ordering.Application.OrderItems.CreateOrderItem;

internal sealed class CreateOrderItemValidator : AbstractValidator<CreateOrderItemCommand>
{
    public CreateOrderItemValidator()
    {
        RuleFor(x => x.OrderId).NotNull();
        RuleFor(x => x.Quantity).NotNull().GreaterThan(0m);
        RuleFor(x => x.Price).NotNull().GreaterThanOrEqualTo(0m);
    }
}
