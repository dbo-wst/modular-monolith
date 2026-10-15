using LanguageExt.Common;

namespace Acme.Core.BuildingBlocks.Domain.Errors;

public sealed record TransactionFailed : AppError
{
    public const string ErrorMessage = "Transaction failed.";

    private TransactionFailed(Exception exception)
        : base(ErrorMessage, ErrorCodes.Internal, Error.New(exception)) { }

    public new static TransactionFailed New(Exception exception) => new(exception);
}
