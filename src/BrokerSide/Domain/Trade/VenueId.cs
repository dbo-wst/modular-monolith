using LanguageExt;

namespace Wst.Trios.Core.BrokerSide.Domain.Trade;

public record VenueId
{
    private VenueId(Guid value)
    {
        Value = value;
    }

    public Guid Value { get; }

    public static Fin<VenueId> MaybeCreate(Guid venueId) => Fin<VenueId>.Succ(new VenueId(venueId));

    public static VenueId From(Guid venueId) => new(venueId);

    public static VenueId? From(Guid? venueId) => venueId.HasValue ? new VenueId(venueId.Value) : null;
}
