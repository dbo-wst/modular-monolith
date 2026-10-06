using LanguageExt;
using Wst.Trios.Core.Shared.Kernel.Functional;

namespace Wst.Trios.Core.BrokerSide.Domain.Order;

public record Side(ESide Value)
{
    public static Fin<Side> MaybeCreate(ESide value) =>new Side(value).ToFin();

    public static Side? From(ESide? value) => value.HasValue ? new Side(value.Value) : null;
}
