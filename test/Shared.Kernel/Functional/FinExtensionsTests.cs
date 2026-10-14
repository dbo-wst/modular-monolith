using LanguageExt;
using LanguageExt.Common;
using Template.Core.Shared.Kernel.Functional;

namespace Template.Core.Shared.Kernel.Test.Functional;

public class FinExtensionsTests
{
    [Fact]
    public void ToUnit_OnSucc_ReturnsSuccUnit()
    {
        var result = Fin<int>.Succ(5).ToUnit();

        Assert.True(result.IsSucc);
    }

    [Fact]
    public void ToUnit_OnFail_PropagatesError()
    {
        var result = Fin<int>.Fail(Error.New("boom")).ToUnit();

        Assert.True(result.IsFail);
        Assert.Equal("boom", ErrorMessage(result));
    }

    [Fact]
    public void MapError_OnSucc_KeepsValue()
    {
        var result = Fin<int>.Succ(5).MapError(() => Error.New("mapped"));

        Assert.True(result.IsSucc);
        result.IfSucc(value => Assert.Equal(5, value));
    }

    [Fact]
    public void MapError_OnFail_ReplacesErrorMessage()
    {
        var result = Fin<int>.Fail(Error.New("original")).MapError(() => Error.New("mapped"));

        Assert.True(result.IsFail);
        Assert.Equal("mapped", ErrorMessage(result));
    }

    [Fact]
    public void Try_Func_OnSucc_ReturnsProjectedValue()
    {
        var result = Fin<int>.Succ(5).Try(value => value * 2);

        Assert.True(result.IsSucc);
        result.IfSucc(value => Assert.Equal(10, value));
    }

    [Fact]
    public void Try_Func_WhenProjectionThrows_ReturnsFail()
    {
        Func<int, int> projection = _ => throw new InvalidOperationException("boom");

        var result = Fin<int>.Succ(5).Try(projection);

        Assert.True(result.IsFail);
        Assert.Contains("Error trying to execute a function", ErrorMessage(result));
    }

    [Fact]
    public void Try_Func_OnFail_DoesNotInvokeProjection()
    {
        var invoked = false;

        var result = Fin<int>.Fail(Error.New("boom")).Try(value =>
        {
            invoked = true;
            return value * 2;
        });

        Assert.False(invoked);
        Assert.True(result.IsFail);
        Assert.Equal("boom", ErrorMessage(result));
    }

    [Fact]
    public void Try_Action_OnSucc_RunsActionAndReturnsSuccUnit()
    {
        var observed = 0;

        var result = Fin<int>.Succ(5).Try(value => observed = value);

        Assert.True(result.IsSucc);
        Assert.Equal(5, observed);
    }

    [Fact]
    public void Try_Action_WhenActionThrows_ReturnsFail()
    {
        var result = Fin<int>.Succ(5).Try(new Action<int>(_ => throw new InvalidOperationException("boom")));

        Assert.True(result.IsFail);
        Assert.Contains("Error trying to execute an action", ErrorMessage(result));
    }

    [Fact]
    public void Try_Action_OnFail_DoesNotInvokeAction()
    {
        var invoked = false;

        var result = Fin<int>.Fail(Error.New("boom")).Try(new Action<int>(_ => invoked = true));

        Assert.False(invoked);
        Assert.True(result.IsFail);
        Assert.Equal("boom", ErrorMessage(result));
    }

    private static string ErrorMessage<T>(Fin<T> fin) =>
        fin.Match(Succ: _ => string.Empty, Fail: error => error.Message);
}
