using LanguageExt;
using LanguageExt.Common;
using Wst.Trios.Core.Shared.Kernel.Functional;

namespace Wst.Trios.Core.Shared.Kernel.Test.Functional;

public class FuncFinAsyncExtensionsTests
{
    [Fact]
    public async Task ToUnitAsync_OnSucc_ReturnsSuccUnit()
    {
        var result = await Succ(5).ToUnitAsync();

        Assert.True(result.IsSucc);
    }

    [Fact]
    public async Task ToUnitAsync_OnFail_PropagatesError()
    {
        var result = await Fail(Error.New("boom")).ToUnitAsync();

        Assert.True(result.IsFail);
        Assert.Equal("boom", ErrorMessage(result));
    }

    [Fact]
    public async Task TryAsync_Func_OnSucc_ReturnsProjectedValue()
    {
        Func<int, string> projection = value => value.ToString();

        var result = await Succ(5).TryAsync(projection);

        Assert.True(result.IsSucc);
        result.IfSucc(value => Assert.Equal("5", value));
    }

    [Fact]
    public async Task TryAsync_Func_WhenProjectionThrows_ReturnsFail()
    {
        Func<int, string> projection = _ => throw new InvalidOperationException("boom");

        var result = await Succ(5).TryAsync(projection);

        Assert.True(result.IsFail);
        Assert.Contains("Error trying to execute a function", ErrorMessage(result));
    }

    [Fact]
    public async Task TryAsync_Action_OnSucc_RunsActionAndReturnsSuccUnit()
    {
        var observed = 0;
        Action<int> action = value => observed = value;

        var result = await Succ(5).TryAsync(action);

        Assert.True(result.IsSucc);
        Assert.Equal(5, observed);
    }

    [Fact]
    public async Task MapErrorAsync_OnFail_ReplacesErrorMessage()
    {
        var result = await Fail(Error.New("original")).MapErrorAsync(() => Error.New("mapped"));

        Assert.True(result.IsFail);
        Assert.Equal("mapped", ErrorMessage(result));
    }

    [Fact]
    public async Task MapErrorAsync_OnSucc_KeepsValue()
    {
        var result = await Succ(5).MapErrorAsync(() => Error.New("mapped"));

        Assert.True(result.IsSucc);
        result.IfSucc(value => Assert.Equal(5, value));
    }

    [Fact]
    public async Task MapAsync_OnSucc_ProjectsValue()
    {
        var result = await Succ(5).MapAsync(value => value * 2);

        Assert.True(result.IsSucc);
        result.IfSucc(value => Assert.Equal(10, value));
    }

    [Fact]
    public async Task MapAsync_OnFail_PropagatesError()
    {
        var result = await Fail(Error.New("boom")).MapAsync(value => value * 2);

        Assert.True(result.IsFail);
        Assert.Equal("boom", ErrorMessage(result));
    }

    [Fact]
    public async Task BindAsync_TaskFin_OnSucc_FlatMapsValue()
    {
        Func<int, Task<Fin<string>>> bind = value => Task.FromResult(Fin<string>.Succ(value.ToString()));

        var result = await Succ(5).BindAsync(bind);

        Assert.True(result.IsSucc);
        result.IfSucc(value => Assert.Equal("5", value));
    }

    [Fact]
    public async Task BindAsync_TaskFin_OnFail_DoesNotInvokeBinder()
    {
        var invoked = false;
        Func<int, Task<Fin<string>>> bind = value =>
        {
            invoked = true;
            return Task.FromResult(Fin<string>.Succ(value.ToString()));
        };

        var result = await Fail(Error.New("boom")).BindAsync(bind);

        Assert.False(invoked);
        Assert.True(result.IsFail);
        Assert.Equal("boom", ErrorMessage(result));
    }

    [Fact]
    public async Task BindAsync_Fin_OnSucc_FlatMapsValue()
    {
        Func<int, Fin<string>> bind = value => Fin<string>.Succ(value.ToString());

        var result = await Succ(5).BindAsync(bind);

        Assert.True(result.IsSucc);
        result.IfSucc(value => Assert.Equal("5", value));
    }

    [Fact]
    public async Task BindAsync_NoArgTaskFin_OnSucc_RunsContinuation()
    {
        Func<Task<Fin<string>>> bind = () => Task.FromResult(Fin<string>.Succ("done"));

        var result = await Succ(5).BindAsync(bind);

        Assert.True(result.IsSucc);
        result.IfSucc(value => Assert.Equal("done", value));
    }

    [Fact]
    public async Task BindForceAsync_OnSucc_RunsContinuationAndReturnsItsResult()
    {
        var invoked = false;
        Func<Task<Fin<string>>> bind = () =>
        {
            invoked = true;
            return Task.FromResult(Fin<string>.Succ("done"));
        };

        var result = await Succ(5).BindForceAsync(bind);

        Assert.True(invoked);
        Assert.True(result.IsSucc);
        result.IfSucc(value => Assert.Equal("done", value));
    }

    [Fact]
    public async Task BindForceAsync_OnFail_RunsContinuationButPreservesOriginalError()
    {
        var invoked = false;
        Func<Task<Fin<string>>> bind = () =>
        {
            invoked = true;
            return Task.FromResult(Fin<string>.Succ("done"));
        };

        var result = await Fail(Error.New("original")).BindForceAsync(bind);

        Assert.True(invoked);
        Assert.True(result.IsFail);
        Assert.Equal("original", ErrorMessage(result));
    }

    [Fact]
    public async Task MatchAsync_TaskFin_OnSucc_RunsSuccessBranch()
    {
        Func<int, Task<Fin<string>>> onSucc = value => Task.FromResult(Fin<string>.Succ($"ok:{value}"));
        Func<Error, Task<Fin<string>>> onFail = error => Task.FromResult(Fin<string>.Succ($"err:{error.Message}"));

        var result = await Succ(5).MatchAsync(onSucc, onFail);

        Assert.True(result.IsSucc);
        result.IfSucc(value => Assert.Equal("ok:5", value));
    }

    [Fact]
    public async Task MatchAsync_TaskFin_OnFail_RunsFailureBranch()
    {
        Func<int, Task<Fin<string>>> onSucc = value => Task.FromResult(Fin<string>.Succ($"ok:{value}"));
        Func<Error, Task<Fin<string>>> onFail = error => Task.FromResult(Fin<string>.Succ($"err:{error.Message}"));

        var result = await Fail(Error.New("boom")).MatchAsync(onSucc, onFail);

        Assert.True(result.IsSucc);
        result.IfSucc(value => Assert.Equal("err:boom", value));
    }

    [Fact]
    public async Task MatchAsync_Task_OnSucc_RunsSuccessBranch()
    {
        Func<int, Task<string>> onSucc = value => Task.FromResult($"ok:{value}");
        Func<Error, Task<string>> onFail = error => Task.FromResult($"err:{error.Message}");

        var result = await Succ(5).MatchAsync(onSucc, onFail);

        Assert.Equal("ok:5", result);
    }

    [Fact]
    public async Task MatchAsync_Sync_OnFail_RunsFailureBranch()
    {
        Func<int, string> onSucc = value => $"ok:{value}";
        Func<Error, string> onFail = error => $"err:{error.Message}";

        var result = await Fail(Error.New("boom")).MatchAsync(onSucc, onFail);

        Assert.Equal("err:boom", result);
    }

    [Fact]
    public async Task MatchAsync_NoArgTaskFin_OnFail_RunsFailureBranch()
    {
        Func<Task<Fin<string>>> onSucc = () => Task.FromResult(Fin<string>.Succ("success"));
        Func<Task<Fin<string>>> onFail = () => Task.FromResult(Fin<string>.Succ("failure"));

        var result = await Fail(Error.New("boom")).MatchAsync(onSucc, onFail);

        Assert.True(result.IsSucc);
        result.IfSucc(value => Assert.Equal("failure", value));
    }

    private static Func<Task<Fin<int>>> Succ(int value) => () => Task.FromResult(Fin<int>.Succ(value));

    private static Func<Task<Fin<int>>> Fail(Error error) => () => Task.FromResult(Fin<int>.Fail(error));

    private static string ErrorMessage<T>(Fin<T> fin) =>
        fin.Match(Succ: _ => string.Empty, Fail: error => error.Message);
}
