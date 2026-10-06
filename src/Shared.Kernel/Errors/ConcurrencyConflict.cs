using LanguageExt.Common;

namespace Wst.Trios.Core.Shared.Kernel.Errors;

public sealed record ConcurrencyConflict : Expected
{
    public const string ErrorMessage = "The record was modified by another process. Reload and retry.";

    private ConcurrencyConflict(Exception exception)
        : base(ErrorMessage, 0, Error.New(exception)) { }

    public new static ConcurrencyConflict New(Exception exception) => new(exception);
}
