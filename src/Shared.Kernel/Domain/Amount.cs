using LanguageExt;
using Wst.Trios.Core.Shared.Kernel.Errors;

namespace Wst.Trios.Core.Shared.Kernel.Domain;

public record Amount(decimal Value)
{
    public static Fin<Amount> Create(decimal value) =>
        value >= 0
            ? Fin<Amount>.Succ(new Amount(value))
            : Fin<Amount>.Fail(ValidationErrors.ValidationError(nameof(Amount)));

    public static Amount From(decimal value) => new(value);
    public static Amount? From(decimal? value) => value.HasValue ? new Amount(value.Value) : null;
}
