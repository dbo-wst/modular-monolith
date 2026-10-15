using LanguageExt.Common;

namespace Template.Core.BuildingBlocks.Domain.Errors;

public sealed record ConcurrencyConflict : ConflictError
{
    public const string ErrorMessage = "The record was modified by another process. Reload and retry.";

    private ConcurrencyConflict(Exception exception)
        : base(ErrorMessage, Error.New(exception)) { }

    public new static ConcurrencyConflict New(Exception exception) => new(exception);
}
