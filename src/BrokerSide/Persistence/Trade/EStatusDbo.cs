using System.Runtime.Serialization;

namespace Wst.Trios.Core.BrokerSide.Persistence.Trade;

public enum EStatusDbo
{
    [EnumMember(Value = "NEW")]
    New,

    [EnumMember(Value = "PARTIALLY_FILLED")]
    PartiallyFilled,

    [EnumMember(Value = "FILLED")]
    Filled,

    [EnumMember(Value = "CANCELLED")]
    Cancelled,

    [EnumMember(Value = "REJECTED")]
    Rejected
}
