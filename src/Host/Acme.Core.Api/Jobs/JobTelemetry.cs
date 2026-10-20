using System.Diagnostics;

namespace Acme.Core.Api.Jobs;

/// <summary>
/// Names shared by the job spans and metrics. The name matches <c>Acme.*</c>, so the source and the meter are exported
/// by <c>AddObservability</c> without further registration.
/// </summary>
internal static class JobTelemetry
{
    public const string Name = "Acme.Core.Api.Jobs";

    public const string JobNameTag = "acme.job.name";
    public const string RunIdTag = "acme.job.run_id";
    public const string OutcomeTag = "acme.outcome";
    public const string ErrorTypeTag = "error.type";

    public static readonly ActivitySource Source = new(Name);
}
