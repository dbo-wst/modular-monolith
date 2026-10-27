using System.Diagnostics;

namespace Acme.Core.Api.Jobs;

/// <summary>
/// Names shared by the job spans, log scopes and metrics: the shared <c>scheduled_job.*</c> contract of the otel stack
/// (wst-cicd <c>otel/TELEMETRY.md</c>). The source and meter name matches <c>Acme.*</c>, so both are exported by
/// <c>AddObservability</c> without further registration.
/// </summary>
internal static class JobTelemetry
{
    public const string Name = "Acme.Core.Api.Jobs";

    public const string JobNameTag = "scheduled_job.name";

    /// <summary>Unique per run: on spans and log lines only, never a metric tag.</summary>
    public const string RunIdTag = "scheduled_job.run_id";

    public const string OutcomeTag = "scheduled_job.outcome";
    public const string ErrorTypeTag = "error.type";

    public static readonly ActivitySource Source = new(Name);

    /// <summary>
    /// Begins a log scope with the given attributes under their dotted names, so log lines carry the same attributes as
    /// the span and the metrics (message template placeholders cannot contain dots). Null values are left out.
    /// </summary>
    public static IDisposable? BeginScope(ILogger logger, params ReadOnlySpan<(string Key, object? Value)> attributes)
    {
        var scope = new Dictionary<string, object>(attributes.Length);
        foreach (var (key, value) in attributes)
        {
            if (value is not null)
            {
                scope[key] = value;
            }
        }

        return logger.BeginScope(scope);
    }
}
