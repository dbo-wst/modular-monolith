using LanguageExt;
using Acme.Core.BuildingBlocks.Domain.Functional;

namespace Acme.Core.Ordering.Domain.ValueObjects;

public record EffectiveDate(DateOnly Value)
{
    public static Fin<EffectiveDate> Create(DateOnly value) => new EffectiveDate(value).ToFin();

    public static EffectiveDate From(DateOnly value) => new(value);

    public static EffectiveDate? From(DateOnly? value) => value.HasValue ? new EffectiveDate(value.Value) : null;

    public static EffectiveDate Today() => new(DateOnly.FromDateTime(DateTime.UtcNow));
}
