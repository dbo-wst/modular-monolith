using System.Runtime.Serialization;

namespace Wst.Trios.Core.BrokerSide.Persistence.Order;

public enum ESideDbo
{
    [EnumMember(Value = "BUY")]
    Buy,

    [EnumMember(Value = "SELL")]
    Sell
}
