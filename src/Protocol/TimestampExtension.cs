using Google.Protobuf.WellKnownTypes;

namespace Wst.Trios.Core.Protocol;

public static class TimestampExtension
{
    public static Timestamp AsTimestamp(this DateOnly date) =>
        Timestamp.FromDateTime(date.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc));

    public static DateOnly AsDateOnly(this Timestamp timestamp) =>
        DateOnly.FromDateTime(timestamp.ToDateTime());
}
