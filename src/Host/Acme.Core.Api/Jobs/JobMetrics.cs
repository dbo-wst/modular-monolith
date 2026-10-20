using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace Acme.Core.Api.Jobs;

/// <summary>
/// Metrics of all jobs (singleton). In Prometheus: <c>acme_job_runs_total</c>, <c>acme_job_duration_seconds</c>,
/// <c>acme_job_active</c>, <c>acme_job_last_success_time_seconds</c> and <c>acme_job_misfires_total</c>, each with the
/// <c>acme_job_name</c> label.
/// </summary>
internal sealed class JobMetrics
{
    private static readonly double[] DurationBuckets = [0.01, 0.05, 0.1, 0.25, 0.5, 1, 2.5, 5, 10, 30, 60, 120, 300];

    private readonly TimeProvider _timeProvider;
    private readonly Counter<long> _runs;
    private readonly Histogram<double> _duration;
    private readonly UpDownCounter<long> _active;
    private readonly Gauge<double> _lastSuccess;
    private readonly Counter<long> _misfires;

    public JobMetrics(IMeterFactory meterFactory, TimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
        var meter = meterFactory.Create(JobTelemetry.Name);

        _runs = meter.CreateCounter<long>("acme.job.runs", "{run}", "Finished job runs by outcome.");
        _duration = meter.CreateHistogram(
            "acme.job.duration", "s", "Duration of a job run.",
            tags: null, advice: new InstrumentAdvice<double> { HistogramBucketBoundaries = DurationBuckets });
        _active = meter.CreateUpDownCounter<long>("acme.job.active", "{run}", "Job runs in progress.");
        _lastSuccess = meter.CreateGauge<double>(
            "acme.job.last_success.time", "s", "Unix time of the last successful run.");
        _misfires = meter.CreateCounter<long>(
            "acme.job.misfires", "{misfire}", "Triggers that missed their fire time and were skipped.");
    }

    public void RunStarted(string jobName) => _active.Add(1, Job(jobName));

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

        _active.Add(-1, Job(jobName));
        _runs.Add(1, tags);
        _duration.Record(duration.TotalSeconds, tags);

        if (outcome == JobOutcome.Success)
        {
            _lastSuccess.Record(_timeProvider.GetUtcNow().ToUnixTimeMilliseconds() / 1000d, Job(jobName));
        }
    }

    public void Misfired(string jobName) => _misfires.Add(1, Job(jobName));

    private static KeyValuePair<string, object?> Job(string jobName) => new(JobTelemetry.JobNameTag, jobName);
}
