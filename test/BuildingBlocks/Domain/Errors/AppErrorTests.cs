using LanguageExt.Common;
using Template.Core.BuildingBlocks.Domain.Errors;
using Template.Core.BuildingBlocks.Domain.Functional;

namespace Template.Core.BuildingBlocks.Domain.Test.Errors;

public sealed class AppErrorTests
{
    [Fact]
    public void WireCodes_AreStable()
    {
        Assert.Equal("not_found", ErrorCodes.NotFound);
        Assert.Equal("validation", ErrorCodes.Validation);
        Assert.Equal("conflict", ErrorCodes.Conflict);
        Assert.Equal("cancelled", ErrorCodes.Cancelled);
        Assert.Equal("internal", ErrorCodes.Internal);
    }

    public static TheoryData<string, string> Cases() => new()
    {
        { nameof(NotFoundError), ErrorCodes.NotFound },
        { nameof(ValidationError), ErrorCodes.Validation },
        { nameof(ConflictError), ErrorCodes.Conflict },
        { nameof(ConcurrencyConflict), ErrorCodes.Conflict },
        { nameof(CancelledError), ErrorCodes.Cancelled },
        { nameof(OperationCancelled), ErrorCodes.Cancelled },
        { nameof(PersistenceFailed), ErrorCodes.Internal },
        { nameof(TransactionFailed), ErrorCodes.Internal },
    };

    private static AppError Create(string type) => type switch
    {
        nameof(NotFoundError) => NotFoundError.New("missing"),
        nameof(ValidationError) => ValidationError.New("invalid"),
        nameof(ConflictError) => ConflictError.New("conflict"),
        nameof(ConcurrencyConflict) => ConcurrencyConflict.New(new InvalidOperationException()),
        nameof(CancelledError) => CancelledError.New("cancelled"),
        nameof(OperationCancelled) => OperationCancelled.New(new OperationCanceledException()),
        nameof(PersistenceFailed) => PersistenceFailed.New(new InvalidOperationException()),
        nameof(TransactionFailed) => TransactionFailed.New(new InvalidOperationException()),
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
    };

    [Theory]
    [MemberData(nameof(Cases))]
    public void EachErrorType_CarriesItsCode_AndIsExpected(string type, string expectedCode)
    {
        var error = Create(type);

        Assert.Equal(expectedCode, error.ErrorCode);
        Assert.True(error.IsExpected);
    }

    [Fact]
    public void ConcurrencyConflict_IsAConflictError()
    {
        Error error = ConcurrencyConflict.New(new InvalidOperationException());

        Assert.IsAssignableFrom<ConflictError>(error);
        Assert.Equal(ConcurrencyConflict.ErrorMessage, error.Message);
    }

    [Fact]
    public void OperationCancelled_IsACancelledError()
    {
        Error error = OperationCancelled.New(new OperationCanceledException());

        Assert.IsAssignableFrom<CancelledError>(error);
        Assert.Equal(OperationCancelled.ErrorMessage, error.Message);
    }

    [Fact]
    public void Factories_KeepHumanReadableMessages()
    {
        Assert.Equal("Order number is required.", ValidationError.New("Order number is required.").Message);
        Assert.Equal("Entity 42 was not found.", NotFoundError.ForEntity(42).Message);
        Assert.Equal("Entity 42 already exists.", ConflictError.AlreadyExists(42).Message);
    }

    [Fact]
    public void ToFin_OnNull_FailsWithValidationError()
    {
        string value = null!;

        var result = value.ToFin();

        var error = Assert.IsType<ValidationError>(result.Match(Succ: _ => null!, Fail: e => e));
        Assert.Equal(ErrorCodes.Validation, error.ErrorCode);
        Assert.Contains(nameof(String), error.Message);
    }
}
