using LanguageExt;
using Template.Core.Shared.Kernel.Errors;

namespace Template.Core.Shared.Kernel.Domain;

public record Price(decimal Value)
{
    public static Fin<Price> Create(decimal value) =>
        value >= 0
            ? Fin<Price>.Succ(new Price(value))
            : Fin<Price>.Fail(ValidationErrors.ValidationError(nameof(Price)));

    public static Price From(decimal value) => new(value);
    public static Price? From(decimal? value) => value.HasValue ? new Price(value.Value) : null;
}
