using LanguageExt;
using Wst.Trios.Core.Shared.Kernel.Functional;

namespace Wst.Trios.Core.BrokerSide.Domain.Order;

public record Status(EStatus Value)
{
    public static Fin<Status> MaybeCreate(EStatus value) =>new Status(value).ToFin();

    public static Status? From(EStatus? value) => value.HasValue ? new Status(value.Value) : null;
}
