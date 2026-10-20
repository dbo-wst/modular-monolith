using FakeItEasy;
using Microsoft.Extensions.Logging.Abstractions;
using Quartz;
using Acme.Core.Api.Jobs;

namespace Acme.Core.Api.Test.Jobs;

public sealed class JobMisfireListenerTests : IDisposable
{
    private readonly JobTestContext _ctx = new();

    public void Dispose() => _ctx.Dispose();

    private ITrigger Trigger()
    {
        var trigger = A.Fake<ITrigger>();
        A.CallTo(() => trigger.JobKey).Returns(new JobKey(_ctx.JobName));
        A.CallTo(() => trigger.Key).Returns(new TriggerKey($"{_ctx.JobName}.cron"));
        return trigger;
    }

    private JobMisfireListener Listener() =>
        new(_ctx.Metrics, _ctx.Time, NullLogger<JobMisfireListener>.Instance);

    [Fact]
    public async Task TriggerMisfired_CountsMisfireForJob()
    {
        await Listener().TriggerMisfired(Trigger());

        var misfire = Assert.Single(_ctx.Misfires.GetMeasurementSnapshot());
        Assert.Equal(_ctx.JobName, misfire.Tags[JobTelemetry.JobNameTag]);
    }

    [Fact]
    public async Task TriggerMisfired_RecordsFirstFireTimeAfterNowAsNextRun()
    {
        // Quartz has not yet moved the trigger past the missed time, so the listener asks for the fire time after now.
        var trigger = Trigger();
        var next = _ctx.Time.GetUtcNow().AddMinutes(5);
        A.CallTo(() => trigger.GetFireTimeAfter(_ctx.Time.GetUtcNow())).Returns(next);

        await Listener().TriggerMisfired(trigger);

        var nextRun = Assert.Single(_ctx.NextRun.GetMeasurementSnapshot());
        Assert.Equal(next.ToUnixTimeSeconds(), nextRun.Value);
        Assert.Equal(_ctx.JobName, nextRun.Tags[JobTelemetry.JobNameTag]);
    }
}
