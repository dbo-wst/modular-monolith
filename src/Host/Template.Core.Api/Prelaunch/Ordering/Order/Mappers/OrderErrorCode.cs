namespace Template.Core.Api.Prelaunch.Ordering.Order.Mappers;

// Classifies a domain error into the stable string code carried by every *Error proto message.
internal static class OrderErrorCode
{
    public static string Classify(LanguageExt.Common.Error error) => error.Message switch
    {
        var m when m.Contains("was not found", StringComparison.OrdinalIgnoreCase) => "not_found",
        var m when m.Contains("is missing", StringComparison.OrdinalIgnoreCase) => "not_found",
        var m when m.Contains("already exists", StringComparison.OrdinalIgnoreCase) => "conflict",
        var m when m.Contains("Validation Error", StringComparison.OrdinalIgnoreCase) => "validation",
        var m when m.Contains("Null Reference", StringComparison.OrdinalIgnoreCase) => "validation",
        var m when m.Contains("cancelled", StringComparison.OrdinalIgnoreCase) => "cancelled",
        _ => "internal"
    };
}
