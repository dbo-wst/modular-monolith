using Acme.Core.Api.Jobs;

namespace Acme.Core.Api.Test.Jobs;

public sealed class JobsOptionsValidatorTests
{
    private readonly JobsOptionsValidator _validator = new();

    [Fact]
    public void Validate_Defaults_IsValid()
    {
        Assert.True(_validator.Validate(new JobsOptions()).IsValid);
    }

    [Theory]
    [InlineData("")]
    [InlineData("*/10 * * * *")]
    [InlineData("every five minutes")]
    public void Validate_InvalidCron_IsInvalidForThatJob(string cron)
    {
        var options = new JobsOptions { Housekeeping = new JobScheduleOptions { Cron = cron } };

        var result = _validator.Validate(options);

        Assert.Contains(result.Errors, e => e.PropertyName == "Housekeeping.Cron");
    }
}
