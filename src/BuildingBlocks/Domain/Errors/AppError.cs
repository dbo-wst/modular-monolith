using LanguageExt;
using LanguageExt.Common;

namespace Template.Core.BuildingBlocks.Domain.Errors;

/// <summary>
/// Base of all typed application errors. Consumers classify by type / <see cref="ErrorCode"/>,
/// never by <see cref="Error.Message"/>, which is free human-readable text.
/// </summary>
public abstract record AppError : Expected
{
    protected AppError(string message, string errorCode, Option<Error> inner = default)
        : base(message, 0, inner)
    {
        ErrorCode = errorCode;
    }

    /// <summary>One of the <see cref="ErrorCodes"/> constants.</summary>
    public string ErrorCode { get; }
}
