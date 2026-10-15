using LanguageExt.Common;

namespace Template.Core.Shared.Kernel.Errors;

public sealed record OperationCancelled : Expected
{
    public const string ErrorMessage = "The operation was cancelled.";

    private OperationCancelled(Exception exception)
        : base(ErrorMessage, 0, Error.New(exception)) { }

    public new static OperationCancelled New(Exception exception) => new(exception);
}
