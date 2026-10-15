using LanguageExt;
using Acme.Core.BuildingBlocks.Domain.Functional;

namespace Acme.Core.Ordering.Domain.ValueObjects;

public record OrderNumber(string Value)
{
    public static Fin<OrderNumber> MaybeCreate(string value) => new OrderNumber(value).ToFin();

    public static OrderNumber? From(string? value) => !string.IsNullOrWhiteSpace(value) ? new OrderNumber(value) : null;

}
