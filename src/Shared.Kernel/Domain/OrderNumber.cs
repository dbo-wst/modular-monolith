using LanguageExt;
using Template.Core.Shared.Kernel.Functional;

namespace Template.Core.Shared.Kernel.Domain;

public record OrderNumber(string Value)
{
    public static Fin<OrderNumber> MaybeCreate(string value) => new OrderNumber(value).ToFin();

    public static OrderNumber? From(string? value) => !string.IsNullOrEmpty(value) ? new OrderNumber(value) : null;

}
