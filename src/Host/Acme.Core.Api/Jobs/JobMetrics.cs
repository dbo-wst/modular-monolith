using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace Acme.Core.Api.Jobs;

/// <summary>
/// Metrics of all jobs (singleton), named after the shared <c>scheduled_job.*</c> contract. In Prometheus:
/// <c>scheduled_job_duration_seconds_*</c> (recorded for every run, so its <c>_count</c> is the run count),
/// <c>scheduled_job_active</c>, <c>scheduled_job_last_start_timestamp_seconds</c>,
/// <c>scheduled_job_last_success_timestamp_seconds</c>,
/// <c>scheduled_job_next_run_timestamp_seconds</c> and <c>scheduled_job_misfires_total</c>, each with the
/// <c>scheduled_job_name</c> label; the duration also has <c>scheduled_job_outcome</c> and, on failure,
/// <c>error_type</c>. It also keeps each job's latest run in memory, so the misfire listener can tell a fire time
/// skipped because the job's own run was still busy from a real misfire.
/// </summary>
internal sealed class JobMetrics
{
    private static readonly double[] DurationBuckets = [0.01, 0.05, 0.1, 0.25, 0.5, 1, 2.5, 5, 10, 30, 60, 120, 300];

    private readonly TimeProvider _timeProvider;
    private readonly Histogram<double> _duration;
    private readonly UpDownCounter<long> _active;
    private readonly Gauge<double> _lastStart;
    private readonly Gauge<double> _lastSuccess;
    private readonly Gauge<double> _nextRun;
    private readonly Counter<long> _misfires;
    private readonly ConcurrentDictionary<string, RunTimes> _runs = new();

    public JobMetrics(IMeterFactory meterFactory, TimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
        var meter = meterFactory.Create(JobTelemetry.Name);

        _duration = meter.CreateHistogram(
            "scheduled_job.duration", "s", "Duration of a job run, by outcome.",
            tags: null, advice: new InstrumentAdvice<double> { HistogramBucketBoundaries = DurationBuckets });
        _active = meter.CreateUpDownCounter<long>("scheduled_job.active", "{run}", "Job runs in progress.");
        _lastStart = meter.CreateGauge<double>(
            "scheduled_job.last_start.timestamp", "s", "Unix time of the start of the latest run.");
        _lastSuccess = meter.CreateGauge<double>(
            "scheduled_job.last_success.timestamp", "s", "Unix time of the last successful run.");
        _nextRun = meter.CreateGauge<double>(
            "scheduled_job.next_run.timestamp", "s", "Unix time of the next scheduled run.");
        _misfires = meter.CreateCounter<long>(
            "scheduled_job.misfires", "{misfire}", "Triggers that missed their fire time and were skipped.");
    }

    public void RunStarted(string jobName)
    {
        var now = _timeProvider.GetUtcNow();
        _runs[jobName] = new RunTimes(now, null);
        _active.Add(1, Job(jobName));
        _lastStart.Record(UnixSeconds(now), Job(jobName));
    }

    public void RunFinished(string jobName, JobOutcome outcome, TimeSpan duration, string? errorType)
    {
        var tags = new TagList
        {
            { JobTelemetry.JobNameTag, jobName },
            { JobTelemetry.OutcomeTag, outcome.ToTagValue() }
        };
        if (errorType is not null)
        {
            tags.Add(JobTelemetry.ErrorTypeTag, errorType);
        }

        var now = _timeProvider.GetUtcNow();
        _runs.AddOrUpdate(jobName, _ => new RunTimes(now, now), (_, run) => run with { Finished = now });
        _active.Add(-1, Job(jobName));
        _duration.Record(duration.TotalSeconds, tags);

        if (outcome == JobOutcome.Succeeded)
        {
            _lastSuccess.Record(UnixSeconds(now), Job(jobName));
        }
    }

    /// <summary>Records when the job runs next; records nothing when the trigger has no further run.</summary>
    public void NextRunScheduled(string jobName, DateTimeOffset? next)
    {
        if (next is { } time)
        {
            _nextRun.Record(UnixSeconds(time), Job(jobName));
        }
    }

    public void Misfired(string jobName) => _misfires.Add(1, Job(jobName));

    /// <summary>Whether a run of the job had started at <paramref name="time"/> and not yet finished.</summary>
    public bool WasRunningAt(string jobName, DateTimeOffset time) =>
        _runs.TryGetValue(jobName, out var run) && run.Started <= time && (run.Finished is null || time <= run.Finished);

    private static KeyValuePair<string, object?> Job(string jobName) => new(JobTelemetry.JobNameTag, jobName);

    private static double UnixSeconds(DateTimeOffset time) => time.ToUnixTimeMilliseconds() / 1000d;

    private sealed record RunTimes(DateTimeOffset Started, DateTimeOffset? Finished);
}
