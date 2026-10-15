using System.Text.Json.Serialization;

namespace Acme.Api.Infrastructure.CoreApi.Prelaunch.Ordering.Order;

public enum EStatus
{
    [JsonStringEnumMemberName("UNSPECIFIED")]
    Unspecified,

    [JsonStringEnumMemberName("NEW")]
    New,

    [JsonStringEnumMemberName("IN_PROGRESS")]
    InProgress,

    [JsonStringEnumMemberName("COMPLETED")]
    Completed,

    [JsonStringEnumMemberName("CANCELLED")]
    Cancelled,

    [JsonStringEnumMemberName("REJECTED")]
    Rejected
}
