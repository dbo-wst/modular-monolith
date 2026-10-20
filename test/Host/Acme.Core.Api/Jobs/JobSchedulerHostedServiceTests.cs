using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Quartz;
using Quartz.Impl.Matchers;
using Acme.Core.Api.Jobs;

namespace Acme.Core.Api.Test.Jobs;

// Quartz keeps schedulers in a process-wide repository by name; one scheduler at a time avoids clashes.
[Collection(nameof(JobSchedulerHostedServiceTests))]
public sealed class JobSchedulerHostedServiceTests
{
    private static ServiceProvider Build(params (string Key, string Value)[] values)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(values.Select(v => KeyValuePair.Create(v.Key, (string?)v.Value)))
            .Build();

        // Quartz caches the logger factory in a static log provider, so it must outlive every test's container.
        return new ServiceCollection()
            .AddSingleton<ILoggerFactory>(NullLoggerFactory.Instance)
            .AddSingleton(typeof(ILogger<>), typeof(NullLogger<>))
            .AddBackgroundJobs(configuration)
            .BuildServiceProvider();
    }

    /// <summary>Starts the hosted service, inspects the running scheduler (null when none was created), stops it.</summary>
    private static async Task<T> WhileStarted<T>(ServiceProvider provider, Func<IScheduler?, Task<T>> inspect)
    {
        var hosted = provider.GetServices<IHostedService>().OfType<JobSchedulerHostedService>().Single();
        await hosted.StartAsync(CancellationToken.None);
        try
        {
            return await inspect(hosted.Scheduler);
        }
        finally
        {
            await hosted.StopAsync(CancellationToken.None);
        }
    }

    [Fact]
    public async Task StartAsync_Disabled_CreatesNoScheduler()
    {
        await using var provider = Build(("Jobs:Enabled", "false"));

        var scheduler = await WhileStarted(provider, Task.FromResult);

        Assert.Null(scheduler);
    }

    [Fact]
    public async Task StartAsync_Defaults_SchedulesBothJobsEveryFiveMinutes()
    {
        await using var provider = Build();

        var triggers = await WhileStarted(provider, async scheduler =>
        {
            var keys = await scheduler!.GetTriggerKeys(GroupMatcher<TriggerKey>.AnyGroup());
            return await Task.WhenAll(keys.Select(key => scheduler.GetTrigger(key)));
        });

        var crons = triggers.OfType<ICronTrigger>().OrderBy(t => t.JobKey.Name).ToList();
        Assert.Equal([HeartbeatJob.Name, HousekeepingJob.Name], crons.Select(t => t.JobKey.Name));
        Assert.All(crons, t => Assert.Equal(JobsOptions.EveryFiveMinutes, t.CronExpressionString));
    }

    [Theory]
    [InlineData(HeartbeatJob.Name, typeof(MonitoredJob<HeartbeatJob>))]
    [InlineData(HousekeepingJob.Name, typeof(MonitoredJob<HousekeepingJob>))]
    public async Task StartAsync_Defaults_WrapsJobAndKeepsDisallowConcurrentExecution(string name, Type expectedType)
    {
        await using var provider = Build();

        var detail = await WhileStarted(provider, scheduler => scheduler!.GetJobDetail(new JobKey(name)));

        Assert.Equal(expectedType, detail!.JobType);
        Assert.True(detail.ConcurrentExecutionDisallowed);
    }

    [Fact]
    public async Task StartAsync_JobDisabled_DoesNotScheduleIt()
    {
        await using var provider = Build(("Jobs:Housekeeping:Enabled", "false"));

        var jobs = await WhileStarted(provider, scheduler => scheduler!.GetJobKeys(GroupMatcher<JobKey>.AnyGroup()));

        Assert.Equal([HeartbeatJob.Name], jobs.Select(k => k.Name));
    }

    [Fact]
    public async Task StartAsync_Enabled_RegistersMisfireListener()
    {
        await using var provider = Build();

        var listeners = await WhileStarted(provider, scheduler =>
            Task.FromResult(scheduler!.ListenerManager.GetTriggerListeners()));

        Assert.Contains(listeners, l => l is JobMisfireListener);
    }

    [Fact]
    public async Task StopAsync_Enabled_ShutsSchedulerDown()
    {
        await using var provider = Build();

        var scheduler = await WhileStarted(provider, Task.FromResult);

        Assert.True(scheduler!.IsShutdown);
    }
}
