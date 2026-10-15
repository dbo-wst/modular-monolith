using LanguageExt;
using LanguageExt.Common;
using Template.Core.BuildingBlocks.Domain.Functional;

namespace Template.Core.BuildingBlocks.Domain.Test.Functional;

public class GeneralTests
{
    [Fact]
    public void ToError_WrapsExceptionWithStandardMessage()
    {
        var exception = new InvalidOperationException("boom");

        var error = exception.ToError();

        Assert.Equal("Error occured.", error.Message);
    }

    [Fact]
    public void Then_AppliesTheProjection()
    {
        var result = 21.Then(value => value * 2);

        Assert.Equal(42, result);
    }

    [Fact]
    public void Unit_Success_ReturnsSucc()
    {
        var result = Unit.Success();

        Assert.True(result.IsSucc);
    }

    [Fact]
    public void Unit_Failure_ReturnsFailWithGivenError()
    {
        var error = Error.New("failed");

        var result = Unit.Failure(error);

        Assert.True(result.IsFail);
        Assert.Equal("failed", ErrorMessage(result));
    }

    [Fact]
    public async Task Unit_SuccessAsync_ReturnsSucc()
    {
        var result = await Unit.SuccessAsync();

        Assert.True(result.IsSucc);
    }

    [Fact]
    public async Task Unit_FailureAsync_ReturnsFailWithGivenError()
    {
        var error = Error.New("failed");

        var result = await Unit.FailureAsync(error);

        Assert.True(result.IsFail);
        Assert.Equal("failed", ErrorMessage(result));
    }

    [Fact]
    public void ToFin_OnNonNullValue_ReturnsSucc()
    {
        var result = "hello".ToFin();

        Assert.True(result.IsSucc);
        result.IfSucc(value => Assert.Equal("hello", value));
    }

    [Fact]
    public void ToFin_OnNullValue_ReturnsFail()
    {
        string value = null!;

        var result = value.ToFin();

        Assert.True(result.IsFail);
    }

    [Fact]
    public void ToFin_WithError_OnNullValue_ReturnsGivenError()
    {
        string value = null!;
        var error = Error.New("custom");

        var result = value.ToFin(error);

        Assert.True(result.IsFail);
        Assert.Equal("custom", ErrorMessage(result));
    }

    [Fact]
    public void ToFin_WithError_OnNonNullValue_ReturnsSucc()
    {
        var result = "hello".ToFin(Error.New("custom"));

        Assert.True(result.IsSucc);
    }

    [Fact]
    public async Task ToFinAsync_OnNonNullValue_ReturnsSucc()
    {
        var result = await "hello".ToFinAsync();

        Assert.True(result.IsSucc);
    }

    [Fact]
    public async Task ToFinAsync_WithError_OnNullValue_ReturnsGivenError()
    {
        string value = null!;
        var error = Error.New("custom");

        var result = await value.ToFinAsync(error);

        Assert.True(result.IsFail);
        Assert.Equal("custom", ErrorMessage(result));
    }

    [Fact]
    public void OptionToFin_OnSome_ReturnsSucc()
    {
        var result = Option<int>.Some(5).ToFin();

        Assert.True(result.IsSucc);
        result.IfSucc(value => Assert.Equal(5, value));
    }

    [Fact]
    public void OptionToFin_OnNone_ReturnsFail()
    {
        var result = Option<int>.None.ToFin();

        Assert.True(result.IsFail);
    }

    [Fact]
    public void OptionToFin_WithError_OnNone_ReturnsGivenError()
    {
        var error = Error.New("missing");

        var result = Option<int>.None.ToFin(error);

        Assert.True(result.IsFail);
        Assert.Equal("missing", ErrorMessage(result));
    }

    [Fact]
    public void OptionToFin_WithError_OnSome_ReturnsSucc()
    {
        var result = Option<int>.Some(5).ToFin(Error.New("missing"));

        Assert.True(result.IsSucc);
    }

    [Fact]
    public async Task OptionToFinAsync_OnSome_ReturnsSucc()
    {
        var result = await Option<int>.Some(5).ToFinAsync();

        Assert.True(result.IsSucc);
    }

    [Fact]
    public async Task TaskOptionToFinAsync_OnSome_ReturnsSucc()
    {
        var source = Task.FromResult(Option<int>.Some(5));

        var result = await source.ToFinAsync();

        Assert.True(result.IsSucc);
        result.IfSucc(value => Assert.Equal(5, value));
    }

    [Fact]
    public async Task TaskOptionToFinAsync_WithError_OnNone_ReturnsGivenError()
    {
        var source = Task.FromResult(Option<int>.None);
        var error = Error.New("missing");

        var result = await source.ToFinAsync(error);

        Assert.True(result.IsFail);
        Assert.Equal("missing", ErrorMessage(result));
    }

    private static string ErrorMessage<T>(Fin<T> fin) =>
        fin.Match(Succ: _ => string.Empty, Fail: error => error.Message);
}
