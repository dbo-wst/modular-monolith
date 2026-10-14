using LanguageExt.Common;

namespace Template.Core.Shared.Kernel.Errors;

public sealed record TransactionFailed : Expected
{
    public const string ErrorMessage = "Transaction failed.";

    private TransactionFailed(Exception exception)
        : base(ErrorMessage, 0, Error.New(exception)) { }

    public new static TransactionFailed New(Exception exception) => new(exception);
}
