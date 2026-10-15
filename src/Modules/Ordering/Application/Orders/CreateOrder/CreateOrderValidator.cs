using FluentValidation;
using Template.Core.Ordering.Contracts.Commands;

namespace Template.Core.Ordering.Application.Orders.CreateOrder;

internal sealed class CreateOrderValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderValidator()
    {
        RuleFor(x => x.Number).NotEmpty().MaximumLength(128);
        RuleFor(x => x.EffectiveDate).NotNull();
    }
}
