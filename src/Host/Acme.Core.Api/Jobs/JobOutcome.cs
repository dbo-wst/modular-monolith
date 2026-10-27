namespace Acme.Core.Api.Jobs;

/// <summary>
/// How a job run ended; the <c>scheduled_job.outcome</c> attribute of the run's span, its summary log line and its
/// metrics.
/// </summary>
internal enum JobOutcome
{
    /// <summary>The run did its work.</summary>
    Succeeded,

    /// <summary>
    /// The run found nothing to do. Kept apart from <see cref="Succeeded"/> so success rates stay meaningful.
    /// </summary>
    Idle,

    /// <summary>The run returned a failed result or threw.</summary>
    Failed,

    /// <summary>The run was stopped by the scheduler shutting down.</summary>
    Cancelled
}

internal static class JobOutcomeExtensions
{
    public static string ToTagValue(this JobOutcome outcome) => outcome switch
    {
        JobOutcome.Succeeded => "succeeded",
        JobOutcome.Idle => "idle",
        JobOutcome.Failed => "failed",
        JobOutcome.Cancelled => "cancelled",
        _ => throw new ArgumentOutOfRangeException(nameof(outcome), outcome, null)
    };
}
