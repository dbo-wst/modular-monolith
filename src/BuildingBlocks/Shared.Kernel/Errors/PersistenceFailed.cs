using LanguageExt.Common;

namespace Template.Core.Shared.Kernel.Errors;

public sealed record PersistenceFailed : Expected
{
    public const string ErrorMessage = "Failed to persist changes.";

    private PersistenceFailed(Exception exception)
        : base(ErrorMessage, 0, Error.New(exception)) { }

    public new static PersistenceFailed New(Exception exception) => new(exception);
}
