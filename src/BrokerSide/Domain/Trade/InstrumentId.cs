using LanguageExt;

namespace Wst.Trios.Core.BrokerSide.Domain.Trade;

public record InstrumentId
{
    private InstrumentId(Guid value)
    {
        Value = value;
    }

    public Guid Value { get; }

    public static Fin<InstrumentId> MaybeCreate(Guid instrumentId) => Fin<InstrumentId>.Succ(new InstrumentId(instrumentId));

    public static InstrumentId From(Guid instrumentId) => new(instrumentId);

    public static InstrumentId? From(Guid? instrumentId) => instrumentId.HasValue ? new InstrumentId(instrumentId.Value) : null;
}
