using LanguageExt.Common;

namespace Template.Core.BuildingBlocks.Domain.Errors;

public sealed record OperationCancelled : CancelledError
{
    public const string ErrorMessage = "The operation was cancelled.";

    private OperationCancelled(Exception exception)
        : base(ErrorMessage, Error.New(exception)) { }

    public new static OperationCancelled New(Exception exception) => new(exception);
}
