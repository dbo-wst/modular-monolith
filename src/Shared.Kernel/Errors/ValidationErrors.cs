using LanguageExt.Common;

namespace Wst.Trios.Core.Shared.Kernel.Errors;

public static class ValidationErrors
{
    public static Error NullError() => Error.New($"Null Reference Error.");
    public static Error NullError(string message) => Error.New($"Null Reference Error. message: {message} ");

    public static Error ValidationError() => Error.New($"Validation Error.");

    public static Error ValidationError(string message) => Error.New($"Validation Error. message: {message} ");
}
