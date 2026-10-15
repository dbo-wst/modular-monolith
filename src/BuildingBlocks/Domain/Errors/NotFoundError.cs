namespace Template.Core.BuildingBlocks.Domain.Errors;

public sealed record NotFoundError : AppError
{
    private NotFoundError(string message) : base(message, ErrorCodes.NotFound) { }

    public new static NotFoundError New(string message) => new(message);

    public static NotFoundError ForEntity<T>(T entity) => new($"Entity {entity} was not found.");
}
