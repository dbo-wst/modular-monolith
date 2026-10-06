using System.Text.Json.Serialization;

namespace Wst.Trios.Api.Infrastructure.CoreApi.Prelaunch.BrokerSide.Order;

public enum EStatus
{
    [JsonStringEnumMemberName("UNSPECIFIED")]
    Unspecified,

    [JsonStringEnumMemberName("NEW")]
    New,

    [JsonStringEnumMemberName("PARTIALLY_FILLED")]
    PartiallyFilled,

    [JsonStringEnumMemberName("FILLED")]
    Filled,

    [JsonStringEnumMemberName("CANCELLED")]
    Cancelled,

    [JsonStringEnumMemberName("REJECTED")]
    Rejected
}
