# 8.11 Background Jobs

Scheduled work runs in Core.Api on Quartz.NET ([ADR 0002](../09-architecture-decisions/0002-metrics-and-background-jobs.md)).
The code lives in `src/Host/Acme.Core.Api/Jobs`.

## Configuration

Section `Jobs` in `appsettings.json`. Override it with environment variables, e.g. `Jobs__Heartbeat__Cron`. The
section is validated on start; an invalid cron stops the host with the option name in the message.

| Key | Default | Meaning |
|---|---|---|
| `Jobs:Enabled` | `true` | `false` creates no scheduler at all (integration tests set this) |
| `Jobs:<Job>:Enabled` | `true` | schedule this job or not |
| `Jobs:<Job>:Cron` | `0 0/5 * * * ?` | Quartz cron with seconds, in the container's time zone (`TZ`) |

The jobs today are placeholders without business logic:

- `Heartbeat` always ends `success`.
- `Housekeeping` always ends `idle`.

Both run every 5 minutes. A trigger that misses its fire time skips that run (`DoNothing`) and is counted as a
misfire. Jobs never run concurrently with themselves (`[DisallowConcurrentExecution]`).

## Telemetry of a run

Jobs are plain Quartz `IJob` classes without telemetry code. The scheduler runs each one inside
`MonitoredJob<TJob>`, which creates the job in the run's DI scope and wraps every run:

- **Trace:** a new root span `Job <name>` with these tags:
  - `acme.job.name`
  - `acme.job.run_id`
  - `acme.outcome`
  - `error.type`, set on failure, which also sets the span status to Error

  Mediator calls, SQL commands and HTTP calls made by the job become its children.
- **Logs:** a scope with `JobName` and `JobRunId` on every line of the run, then one summary line `Job <name> finished:
  <outcome> in <ms> ms` (EventId 2001). An exception is logged once as Error (2002), then rethrown to Quartz as a
  `JobExecutionException` without refire. Failed mediator results are already logged by `FailureLoggingBehavior`.
  The scheduler logs `JobScheduled` (2012) with the first run time at start, and misfires as Warning (2003).
- **Metrics** (meter `Acme.Core.Api.Jobs`). Every series has the label `acme_job_name`:

| OTel instrument | Prometheus series | Type | Extra labels |
|---|---|---|---|
| `acme.job.runs` | `acme_job_runs_total` | counter | `acme_outcome`, `error_type` |
| `acme.job.duration` | `acme_job_duration_seconds_*` | histogram (0.01 s – 300 s, exemplars) | `acme_outcome`, `error_type` |
| `acme.job.active` | `acme_job_active` | up-down counter | – |
| `acme.job.last_success.time` | `acme_job_last_success_time_seconds` | gauge (unix seconds) | – |
| `acme.job.misfires` | `acme_job_misfires_total` | counter | – |

The job reports its outcome through `context.Result`: nothing set means `success`, `JobOutcome.Idle` means
`idle`, and an `Error` means `failed`.

Outcomes:
- `success`: the run did its work
- `idle`: nothing to do; not counted as success, so the last-success time does not move
- `failed`: the job set an `Error` (e.g. a failed mediator result), with `error.type` = its error code, or the job threw,
  with `error.type` = the exception type
- `cancelled`: scheduler shutdown

Monitoring lives in wst-cicd `otel`:
- the **Acme** dashboard (folder WST)
- the alert rules in group `acme-jobs`: failed run, stale job, misfire, no runs at all

## Adding a job

1. Create `Jobs/<Name>Job.cs`:
   - a plain `internal sealed` Quartz `IJob`, marked `[DisallowConcurrentExecution]`; the scheduler carries the
     attribute over to the wrapper
   - has a `public const string Name`
   - needs no registration: it is created per run in its own DI scope
2. Implement `Execute`. Inject `IMediator`, send a module command and report the result:

   ```csharp
   var result = await mediator.SendAsync(new SomeCommand(), context.CancellationToken);
   context.Result = result.Match<object?>(Succ: r => r.Processed == 0 ? JobOutcome.Idle : null, Fail: e => e);
   ```

   Never put business logic in the job.
3. Add a `JobScheduleOptions` property to `JobsOptions` and validate it in `JobsOptionsValidator`. Schedule the job in
   `JobSchedulerHostedService.StartAsync`.
4. Add the job to `appsettings.json`, and add tests in `test/Host/Acme.Core.Api`.
