using Microsoft.Extensions.Options;
using Acme.Core.Api.Jobs;

namespace Acme.Core.Api.IntegrationTest.Jobs;

[Collection(TestsCollectionDefinition.CollectionName)]
public sealed class BackgroundJobsTests(CoreApiFactory factory) : IClassFixture<CoreApiFactory>
{
    [Fact]
    public void Host_JobsDisabledInConfiguration_StartsNoScheduler()
    {
        var services = factory.Services;

        var hosted = services.GetServices<IHostedService>().OfType<JobSchedulerHostedService>().Single();

        Assert.False(services.GetRequiredService<IOptions<JobsOptions>>().Value.Enabled);
        Assert.Null(hosted.Scheduler);
    }
}
