# 2. Metrics Export and Background Jobs

## Status

Accepted

## Context

Core.Api needs scheduled background work (jobs). A job has no caller who sees it fail, so it has to be watched from
the outside: did it run, how did it end, how long did it take, and when did it last succeed. Logs and traces answer
"what happened in this run". They do not answer "has this job not succeeded for 30 minutes", which is what an alert
needs. Until now `BuildingBlocks.Observability` exported logs and traces only, and metrics needed a recorded
decision. The `otel` stack of wst-cicd already ingests metrics (OTLP → Prometheus → Grafana) and documents the job
pattern in `otel/DOTNET.md` §6 and `otel/METRICS.md`.

## Decision

- **Metrics are exported.** `AddObservability` adds `WithMetrics` with an OTLP exporter, under the same switch as
  logs and traces (`OTEL_EXPORTER_OTLP_ENDPOINT`). Exported are Acme's own meters (`Acme.*`) and the .NET runtime
  meter `System.Runtime` (GC, heap, thread pool, lock contention, exceptions, CPU), which feeds the generic Runtime
  dashboard of the otel stack. ASP.NET Core and HTTP meters are still not exported: request rates and latencies keep
  coming from Tempo span metrics, so the series count stays small. Exemplars are trace-based, so a histogram bucket
  links to a trace.
- Custom `ActivitySource`s named `Acme.*` are traced without further registration.
- **Every signal identifies the build and the instance.** Besides `service.name`, `service.namespace` and
  `deployment.environment.name`, the resource carries `service.version` (the entry assembly's informational version
  without the `+<commit>` suffix; the image build passes the image version as `-p:Version`, `0.0.0` when unknown) and
  `service.instance.id` (the machine name, i.e. the container's host name). Pairs in `OTEL_RESOURCE_ATTRIBUTES`
  override both.
- **Background jobs run on Quartz.NET in Core.Api**, with an in-memory job store. Core.Api runs as a single
  instance, so no clustering or persistent store is needed. `Jobs:Enabled` switches the scheduler off, and each job
  has its own `Enabled` and `Cron`.
- **Jobs emit the shared `scheduled_job.*` contract.** Telemetry of scheduled jobs is not Acme-specific: the otel
  stack defines one contract for every application (wst-cicd `otel/TELEMETRY.md`), so its dashboards and alert rules
  work for any application that emits it. Acme uses exactly these names; the meter stays `Acme.Core.Api.Jobs`.
  - attributes: `scheduled_job.name`, `scheduled_job.run_id` (spans and logs only, never a metric attribute),
    `scheduled_job.outcome` (`succeeded`, `idle`, `failed`, `cancelled`) and the semantic-convention `error.type`
  - instruments: `scheduled_job.duration` (histogram, recorded for every run, so its count is the run count),
    `scheduled_job.active`, `scheduled_job.last_success.timestamp`, `scheduled_job.next_run.timestamp` and
    `scheduled_job.misfires`
  - Quartz's built-in tracing (`AddSource("Quartz")`) stays off: it writes unprefixed `job.*` attributes outside the
    contract.
- **Jobs are plain Quartz `IJob` classes.** The scheduler runs each one inside `MonitoredJob<TJob>`, which gives each
  run:
  - one root span `Job <name>`
  - a log scope with `scheduled_job.name` and `scheduled_job.run_id`
  - one summary log line, with `scheduled_job.outcome` and `error.type` in its scope
  - the `scheduled_job.*` metrics, with the outcome and `error.type` on the duration
- Jobs contain no business logic. Real work is a module command sent through `IMediator`, like a gRPC service method.
- Monitoring (dashboards and alert rules) lives in wst-cicd `otel` and is generic: it filters by environment,
  application (`service.namespace`), service (`service.name`), instance and job.

## Consequences

- Prometheus gets `scheduled_job_*` series, low cardinality: one label set per job and outcome. The runtime meter adds
  a fixed set of `dotnet_*` series per instance.
- Alerts can detect failed, stale, missing, overdue and misfired jobs.
- A new application gets the job dashboards and alerts by emitting the same contract, without changes to the stack.
- A scale-out of Core.Api would run every job once per instance. That then needs a persistent, clustered Quartz store
  or a separate worker host, and a new ADR.
- New metrics must follow `otel/METRICS.md`:
  - meters are named `Acme.*`
  - attributes are bounded
  - no ids, user data or messages as attributes
