using Wst.Trios.Core.Shared.Kernel.Functional;

namespace Wst.Trios.Core.Shared.Kernel.Test.Functional;

public class AdvancedTests
{
    [Fact]
    public void Applying_Arity1_BindsTheSingleArgument()
    {
        Func<int, int> func = x => x + 1;

        var applied = func.Applying(41);

        Assert.Equal(42, applied());
    }

    [Fact]
    public void Applying_Arity2_BindsFirstArgument()
    {
        Func<int, int, int> func = (a, b) => a + b;

        var applied = func.Applying(3);

        Assert.Equal(10, applied(7));
    }

    [Fact]
    public void Applying_Arity3_BindsFirstArgument()
    {
        Func<int, int, int, int> func = (a, b, c) => a + b + c;

        var applied = func.Applying(1);

        Assert.Equal(6, applied(2, 3));
    }

    [Fact]
    public void Applying_Arity4_BindsFirstArgument()
    {
        Func<int, int, int, int, int> func = (a, b, c, d) => a + b + c + d;

        var applied = func.Applying(1);

        Assert.Equal(10, applied(2, 3, 4));
    }

    [Fact]
    public void Applying_Arity5_BindsFirstArgument()
    {
        Func<int, int, int, int, int, int> func = (a, b, c, d, e) => a + b + c + d + e;

        var applied = func.Applying(1);

        Assert.Equal(15, applied(2, 3, 4, 5));
    }

    [Fact]
    public void Currying_Arity2_ProducesNestedSingleArgumentFunctions()
    {
        Func<int, int, int> func = (a, b) => a + b;

        var curried = func.Currying();

        Assert.Equal(5, curried(2)(3));
    }

    [Fact]
    public void Currying_Arity3_ProducesNestedSingleArgumentFunctions()
    {
        Func<int, int, int, int> func = (a, b, c) => a + b + c;

        var curried = func.Currying();

        Assert.Equal(6, curried(1)(2)(3));
    }
}
