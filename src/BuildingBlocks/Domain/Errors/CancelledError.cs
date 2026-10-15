using LanguageExt;
using LanguageExt.Common;

namespace Acme.Core.BuildingBlocks.Domain.Errors;

public record CancelledError : AppError
{
    protected CancelledError(string message, Option<Error> inner = default)
        : base(message, ErrorCodes.Cancelled, inner) { }

    public new static CancelledError New(string message) => new(message);
}
