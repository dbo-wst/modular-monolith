using LanguageExt.Common;

namespace Template.Core.Shared.Kernel.Errors;

public static class RequiredFieldError
{
    public static Error Missing(string field) => Error.New($"Required field '{field}' is missing.");
}
