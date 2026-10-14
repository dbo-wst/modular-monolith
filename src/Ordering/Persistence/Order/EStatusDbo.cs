using System.Runtime.Serialization;

namespace Template.Core.Ordering.Persistence.Order;

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
