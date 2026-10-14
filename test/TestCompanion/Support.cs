using LanguageExt.Common;


namespace Template.Core.TestCompanion;

public static partial class Support
{
    public static Bogus.Faker NewFaker() => new();

    public static Action<Error> AssertError(Error expectedError) => actualError =>
    {
        Assert.NotNull(actualError);
        Assert.Equal(expectedError.Message, actualError.Message);
    };
}

