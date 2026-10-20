using FakeItEasy;
using Microsoft.Extensions.Logging.Abstractions;
using Quartz;
using Acme.Core.Api.Jobs;

namespace Acme.Core.Api.Test.Jobs;

public sealed class JobMisfireListenerTests : IDisposable
{
    private readonly JobTestContext _ctx = new();

    public void Dispose() => _ctx.Dispose();

    [Fact]
    public async Task TriggerMisfired_CountsMisfireForJob()
    {
        var trigger = A.Fake<ITrigger>();
        A.CallTo(() => trigger.JobKey).Returns(new JobKey(_ctx.JobName));
        A.CallTo(() => trigger.Key).Returns(new TriggerKey($"{_ctx.JobName}.cron"));
        var listener = new JobMisfireListener(_ctx.Metrics, NullLogger<JobMisfireListener>.Instance);

        await listener.TriggerMisfired(trigger);

        var misfire = Assert.Single(_ctx.Misfires.GetMeasurementSnapshot());
        Assert.Equal(_ctx.JobName, misfire.Tags[JobTelemetry.JobNameTag]);
    }
}
