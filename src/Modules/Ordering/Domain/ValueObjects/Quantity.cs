using LanguageExt;
using Template.Core.Shared.Kernel.Functional;

namespace Template.Core.Ordering.Domain.ValueObjects;

public record Quantity(decimal Value)
{
    public static Fin<Quantity> Create(decimal value) =>new Quantity(value).ToFin();

    public static Quantity? From(decimal? value) => value.HasValue ? new Quantity(value.Value) : null;
}
