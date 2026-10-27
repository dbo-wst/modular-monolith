using LanguageExt.Common;

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

    /// <summary>
    /// Classifies an error by type only: the first <see cref="AppError"/> found (the error itself, the members of a
    /// <see cref="ManyErrors"/> in order, or an inner error) decides; anything untyped is <see cref="Internal"/>.
    /// </summary>
    public static string Of(Error error) => FindAppError(error)?.ErrorCode ?? Internal;

    private static AppError? FindAppError(Error error) => error switch
    {
        AppError appError => appError,
        ManyErrors many => many.Errors.Select(FindAppError).FirstOrDefault(e => e is not null),
        _ => error.Inner.MatchUnsafe(FindAppError, () => null)
    };
}
