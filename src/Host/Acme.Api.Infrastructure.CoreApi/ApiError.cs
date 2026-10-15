using LanguageExt.Common;

namespace Acme.Api.Infrastructure.CoreApi;

public sealed record ApiError : Expected
{
    public string ErrorCode { get; }

    private ApiError(string errorCode, string message)
        : base(message, 0) => ErrorCode = errorCode;

    public static ApiError New(string errorCode, string message) => new(errorCode, message);
}
