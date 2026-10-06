using System.Text.Json.Serialization;

namespace Wst.Trios.Api.Infrastructure.CoreApi.Prelaunch.BrokerSide.Order;

public enum ESide
{
    [JsonStringEnumMemberName("UNSPECIFIED")]
    Unspecified,

    [JsonStringEnumMemberName("BUY")]
    Buy,

    [JsonStringEnumMemberName("SELL")]
    Sell
}
