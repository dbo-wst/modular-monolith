using LanguageExt.Common;

namespace Template.Core.BuildingBlocks.Domain.Errors;

public sealed record PersistenceFailed : AppError
{
    public const string ErrorMessage = "Failed to persist changes.";

    private PersistenceFailed(Exception exception)
        : base(ErrorMessage, ErrorCodes.Internal, Error.New(exception)) { }

    public new static PersistenceFailed New(Exception exception) => new(exception);
}
