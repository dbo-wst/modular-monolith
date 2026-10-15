using System.Runtime.Serialization;

namespace Acme.Core.Ordering.Persistence.Orders;

public enum EStatusDbo
{
    [EnumMember(Value = "NEW")]
    New,

    [EnumMember(Value = "IN_PROGRESS")]
    InProgress,

    [EnumMember(Value = "COMPLETED")]
    Completed,

    [EnumMember(Value = "CANCELLED")]
    Cancelled,

    [EnumMember(Value = "REJECTED")]
    Rejected
}
