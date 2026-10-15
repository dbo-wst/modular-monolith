using LanguageExt;
using Acme.Core.BuildingBlocks.Domain.Errors;

namespace Acme.Core.Ordering.Domain.ValueObjects;

public record Price(decimal Value)
{
    public static Fin<Price> Create(decimal value) =>
        value >= 0
            ? Fin<Price>.Succ(new Price(value))
            : Fin<Price>.Fail(ValidationError.New("Price must not be negative."));

    public static Price From(decimal value) => new(value);
    public static Price? From(decimal? value) => value.HasValue ? new Price(value.Value) : null;
}
