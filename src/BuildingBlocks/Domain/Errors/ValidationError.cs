namespace Acme.Core.BuildingBlocks.Domain.Errors;

public sealed record ValidationError : AppError
{
    private ValidationError(string message) : base(message, ErrorCodes.Validation) { }

    public new static ValidationError New(string message) => new(message);
}
