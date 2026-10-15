using LanguageExt.Common;
using Template.Core.BuildingBlocks.Domain.Errors;

namespace Template.Core.Api.Prelaunch;

// Classifies a domain error into the stable string code carried by every *Error proto message.
// Classification is by type only: the first AppError found (the error itself, the members of a
// ManyErrors in order, or an inner error) decides; anything untyped is "internal".
internal static class ErrorCodeClassifier
{
    public static string Classify(Error error) => FindAppError(error)?.ErrorCode ?? ErrorCodes.Internal;

    private static AppError? FindAppError(Error error) => error switch
    {
        AppError appError => appError,
        ManyErrors many => many.Errors.Select(FindAppError).FirstOrDefault(e => e is not null),
        _ => error.Inner.MatchUnsafe(FindAppError, () => null)
    };
}
