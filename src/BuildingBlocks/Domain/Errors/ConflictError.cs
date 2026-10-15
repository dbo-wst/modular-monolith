using LanguageExt;
using LanguageExt.Common;

namespace Acme.Core.BuildingBlocks.Domain.Errors;

/// <summary>The request conflicts with the current state (duplicate entity, concurrent modification, ...).</summary>
public record ConflictError : AppError
{
    protected ConflictError(string message, Option<Error> inner = default)
        : base(message, ErrorCodes.Conflict, inner) { }

    public new static ConflictError New(string message) => new(message);

    public static ConflictError AlreadyExists<T>(T entity) => new($"Entity {entity} already exists.");
}
