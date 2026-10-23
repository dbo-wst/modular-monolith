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

- `Heartbeat` always ends `succeeded`.
- `Housekeeping` always ends `idle`.

Both run every 5 minutes. A trigger that misses its fire time skips that run (`DoNothing`). It is counted as a
misfire only when the job was not running at the missed time; a fire time that falls into the job's own run is
skipped by design and logged at Debug (2004). Jobs never run concurrently with themselves
(`[DisallowConcurrentExecution]`).

## Telemetry of a run

Jobs are plain Quartz `IJob` classes without telemetry code. The scheduler runs each one inside
`MonitoredJob<TJob>`, which creates the job in the run's DI scope and wraps every run. All names follow the shared
`scheduled_job.*` contract of the otel stack (wst-cicd `otel/TELEMETRY.md`), so the generic dashboards and alert rules
there work for Acme without Acme-specific queries
([ADR 0002](../09-architecture-decisions/0002-metrics-and-background-jobs.md)).

| Attribute | Span | Log scope | Metrics |
|---|---|---|---|
| `scheduled_job.name` | yes | every line of the run, the scheduler's and the misfire lines | every instrument |
| `scheduled_job.run_id` (Quartz fire instance id) | yes | every line of the run | never (unbounded) |
| `scheduled_job.outcome` | yes | summary line | `scheduled_job.duration` |
| `error.type` (on failure only) | yes, status Error | summary line | `scheduled_job.duration` |

- **Trace:** a new root span `Job <name>` with the attributes above. Mediator calls, SQL commands and HTTP calls made
  by the job become its children. Quartz's own tracing stays off; it would add unprefixed `job.*` attributes.
- **Logs:** the attributes come from log scopes under their dotted names (message template placeholders cannot
  contain dots). One summary line `Job run finished in <ms> ms` (EventId 2001) per run. An exception is logged once
  as Error, `Job run threw` (2002), then rethrown to Quartz as a `JobExecutionException` without refire. Failed
  mediator results are already logged by `FailureLoggingBehavior`. The scheduler logs `JobScheduled` (2012) with the
  first run time at start, and misfires as Warning (2003).
- **Metrics** (meter `Acme.Core.Api.Jobs`). Every series has the label `scheduled_job_name`:

| OTel instrument | Prometheus series | Type | Recorded |
|---|---|---|---|
| `scheduled_job.duration` | `scheduled_job_duration_seconds_*` | histogram (0.01 s – 300 s, exemplars) with `scheduled_job_outcome` and `error_type` | at the end of every run; `_count` is the run count |
| `scheduled_job.active` | `scheduled_job_active` | up-down counter | +1 at the start, -1 at the end of a run |
| `scheduled_job.last_start.timestamp` | `scheduled_job_last_start_timestamp_seconds` | gauge (unix seconds) | at the start of every run |
| `scheduled_job.last_success.timestamp` | `scheduled_job_last_success_timestamp_seconds` | gauge (unix seconds) | after a `succeeded` run |
| `scheduled_job.next_run.timestamp` | `scheduled_job_next_run_timestamp_seconds` | gauge (unix seconds) | when scheduled (first run), at the start of each run and on a misfire |
| `scheduled_job.misfires` | `scheduled_job_misfires_total` | counter | on a misfire while the job was not running |

The job reports its outcome through `context.Result`: nothing set means `succeeded`, `JobOutcome.Idle` means
`idle`, and an `Error` means `failed`.

Outcomes:
- `succeeded`: the run did its work
- `idle`: nothing to do; not counted as success, so the last-success time does not move
- `failed`: the job set an `Error` (e.g. a failed mediator result), with `error.type` = its error code, or the job threw,
  with `error.type` = the exception type
- `cancelled`: scheduler shutdown

Monitoring lives in wst-cicd `otel` and is not Acme-specific; select environment, application `acme`, service
`acme-core-api` and the job:
- the dashboard **Scheduled jobs** (folder Scheduled jobs)
- the alert rules of group `scheduled-jobs`: failed run, overdue job (the next run time has passed and no run is in
  progress), running long (a run still active after an hour), misfire

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
