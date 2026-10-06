using LanguageExt;

namespace Wst.Trios.Core.BrokerSide.Domain.Trade;

public record BrokerId
{
    private BrokerId(Guid value)
    {
        Value = value;
    }

    public Guid Value { get; }

    public static Fin<BrokerId> MaybeCreate(Guid brokerId) => Fin<BrokerId>.Succ(new BrokerId(brokerId));
    public static BrokerId From(Guid brokerId) => new(brokerId);

    public static BrokerId? From(Guid? brokerId) => brokerId.HasValue ? new BrokerId(brokerId.Value) : null;
}
