# Jobs

Background jobs of Core.Api on Quartz.NET (namespace `Acme.Core.Api.Jobs`). Each run is traced, logged and measured
(ADR 0002). Concept and metric reference: `doc/08-cross-cutting-concepts/11-background-jobs.md`.

## Contents

| Item | Kind | Description |
|---|---|---|
| `ServiceCollectionExtensions.cs` | static class | `AddBackgroundJobs(configuration)`. It binds and validates `Jobs`, and registers the Quartz scheduler (in-memory), `JobMetrics`, `JobMisfireListener` and `JobSchedulerHostedService`. |
| `JobSchedulerHostedService.cs` | hosted service | At start, reads `JobsOptions`, schedules the enabled jobs (cron, misfire = skip), adds the misfire listener and starts the scheduler. At stop, shuts down and waits for running jobs. When `Jobs:Enabled` is false it creates no scheduler. |
| `MonitoredJob.cs` | `MonitoredJob<TJob> : IJob` | The type Quartz actually runs. It creates the plain job `TJob` in the run's DI scope, gives each run a root span `Job <name>`, a log scope `scheduled_job.name`/`scheduled_job.run_id`, a summary log line with `scheduled_job.outcome` (and `error.type`) in its scope and the metrics, records the next fire time, and takes the outcome from `context.Result` (nothing = `succeeded`, `JobOutcome.Idle`, an `Error` = `failed`) or from an exception. |
| `JobOutcome.cs` | enum | `Succeeded`, `Idle`, `Failed`, `Cancelled`; the lowercase name is the `scheduled_job.outcome` attribute. |
| `JobMetrics.cs` | singleton | The `scheduled_job.*` instruments on meter `Acme.Core.Api.Jobs`: `duration` (every run), `active`, `last_success.timestamp`, `next_run.timestamp`, `misfires`. |
| `JobTelemetry.cs` | static class | ActivitySource/meter name, the `scheduled_job.*` attribute names and `BeginScope` (log scope with dotted keys). |
| `JobMisfireListener.cs` | Quartz trigger listener | Counts and logs misfires and records the next regular run. |
| `HeartbeatJob.cs`, `HousekeepingJob.cs` | jobs | Plain `IJob` placeholders without business logic. `Heartbeat` ends `succeeded`, `Housekeeping` sets `context.Result = JobOutcome.Idle`. Both run every 5 minutes. |
| `JobsOptions.cs`, `JobsOptionsValidator.cs` | options | Section `Jobs`: `Enabled`, plus `<Job>:Enabled` and `<Job>:Cron` per job. The validator checks each cron with `CronExpression.IsValidExpression`. |

## How it works

- `Program.cs` calls `AddBackgroundJobs` after the modules.
- The schedule is read when the host starts, not when services are registered, so test configuration from
  `WebApplicationFactory` is honoured.
- Quartz runs `MonitoredJob<TJob>` in its own DI scope and that creates `TJob` there, so a job can inject scoped
  services such as `IMediator` and needs no registration. `[DisallowConcurrentExecution]` on the job is copied to the
  scheduled wrapper.
- The meter and source names match `Acme.*`, so `AddObservability` exports them without extra registration.
- Attribute and instrument names are the shared `scheduled_job.*` contract (wst-cicd `otel/TELEMETRY.md`), so the
  generic Scheduled jobs dashboard and alert rules of the otel stack work unchanged. Quartz's own tracing
  (`AddSource("Quartz")`) is not enabled: it writes unprefixed `job.*` attributes.
- Quartz's own logs are limited to Warning (`Serilog:MinimumLevel:Override:Quartz`).

## Dependencies
- References: `Quartz.Extensions.Hosting`, `FluentValidation`, `Acme.Core.BuildingBlocks.Application`
  (`ValidateFluentValidation`) and `Acme.Core.BuildingBlocks.Domain` (`ErrorCodes`).
- Used by: `Program.cs`. Tests are in `test/Host/Acme.Core.Api/Jobs` and
  `test_integration/Host/Acme.Core.Api.IntegrationTest/Jobs`.

## Extending
See "Adding a job" in `doc/08-cross-cutting-concepts/11-background-jobs.md`. Real work goes through `IMediator` with a
module command; never put business logic in a job.
