namespace Acme.Core.Api.Jobs;

/// <summary>Section <c>Jobs</c>: the scheduler switch and one schedule per job.</summary>
public sealed class JobsOptions
{
    public const string SectionName = "Jobs";

    /// <summary>Name of the Quartz scheduler (unique in Quartz's process-wide scheduler repository).</summary>
    public const string SchedulerName = "acme-core-api";

    /// <summary>Quartz cron for "every 5 minutes, on the minute".</summary>
    public const string EveryFiveMinutes = "0 0/5 * * * ?";

    /// <summary>Off: no scheduler is started at all (integration tests, maintenance).</summary>
    public bool Enabled { get; set; } = true;

    public JobScheduleOptions Heartbeat { get; set; } = new();

    public JobScheduleOptions Housekeeping { get; set; } = new();
}

/// <summary>Schedule of one job.</summary>
public sealed class JobScheduleOptions
{
    public bool Enabled { get; set; } = true;

    /// <summary>Quartz cron expression (with seconds), evaluated in the container's time zone.</summary>
    public string Cron { get; set; } = JobsOptions.EveryFiveMinutes;
}
