using LanguageExt.Common;
using Acme.Core.BuildingBlocks.Domain.Errors;

namespace Acme.Core.BuildingBlocks.Domain.Test.Errors;

public sealed class ErrorCodesTests
{
    public static TheoryData<string, string> Cases() => new()
    {
        { "typed", ErrorCodes.NotFound },
        { "many, first typed member decides", ErrorCodes.Validation },
        { "inner typed error", ErrorCodes.Conflict },
        { "exception-wrapping typed error", ErrorCodes.Internal },
        { "untyped", ErrorCodes.Internal },
        { "exception", ErrorCodes.Internal },
    };

    private static Error Create(string shape) => shape switch
    {
        "typed" => NotFoundError.New("missing"),
        "many, first typed member decides" => Error.Many(Error.New("untyped"), ValidationError.New("invalid"), ConflictError.New("taken")),
        "inner typed error" => Error.New("outer", (Error)ConflictError.New("taken")),
        "exception-wrapping typed error" => PersistenceFailed.New(new InvalidOperationException()),
        "untyped" => Error.New("untyped"),
        "exception" => Error.New(new InvalidOperationException()),
        _ => throw new ArgumentOutOfRangeException(nameof(shape), shape, null)
    };

    [Theory]
    [MemberData(nameof(Cases))]
    public void Of_ClassifiesByType(string shape, string expectedCode) =>
        Assert.Equal(expectedCode, ErrorCodes.Of(Create(shape)));
}
