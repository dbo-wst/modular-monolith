using LanguageExt.Common;
using Acme.Core.BuildingBlocks.Domain.Errors;

namespace Acme.Core.Api.Prelaunch;

// Classifies a domain error into the stable string code carried by every *Error proto message.
// Classification is by type only (see ErrorCodes.Of); anything untyped is "internal".
internal static class ErrorCodeClassifier
{
    public static string Classify(Error error) => ErrorCodes.Of(error);
}
