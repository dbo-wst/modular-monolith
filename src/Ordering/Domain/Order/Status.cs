using LanguageExt;
using Template.Core.Shared.Kernel.Functional;

namespace Template.Core.Ordering.Domain.Order;

public record Status(EStatus Value)
{
    public static Fin<Status> MaybeCreate(EStatus value) =>new Status(value).ToFin();

    public static Status? From(EStatus? value) => value.HasValue ? new Status(value.Value) : null;
}
