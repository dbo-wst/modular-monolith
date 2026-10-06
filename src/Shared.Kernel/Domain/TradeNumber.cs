using LanguageExt;
using Wst.Trios.Core.Shared.Kernel.Functional;

namespace Wst.Trios.Core.Shared.Kernel.Domain;

public record TradeNumber(string Value)
{
    public static Fin<TradeNumber> MaybeCreate(string value) => new TradeNumber(value).ToFin();

    public static TradeNumber? From(string? value) => !string.IsNullOrEmpty(value) ? new TradeNumber(value) : null;

}
