namespace Acme.Core.BuildingBlocks.Domain.Errors;

/// <summary>
/// Stable, transport-agnostic error codes. Hosts put these on the wire (gRPC *Error.Code) and the
/// HTTP gateway maps them to status codes, so the values must never change.
/// </summary>
public static class ErrorCodes
{
    public const string NotFound = "not_found";
    public const string Validation = "validation";
    public const string Conflict = "conflict";
    public const string Cancelled = "cancelled";
    public const string Internal = "internal";
}
