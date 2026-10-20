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
  logs and traces (`OTEL_EXPORTER_OTLP_ENDPOINT`). Only Acme's own meters (`Acme.*`) are exported, not runtime,
  ASP.NET Core or HTTP metrics. Request rates and latencies keep coming from Tempo span metrics, so the series count
  stays small. Exemplars are trace-based, so a histogram bucket links to a trace.
- Custom `ActivitySource`s named `Acme.*` are traced without further registration.
- **Background jobs run on Quartz.NET in Core.Api**, with an in-memory job store. Core.Api runs as a single
  instance, so no clustering or persistent store is needed. `Jobs:Enabled` switches the scheduler off, and each job
  has its own `Enabled` and `Cron`.
- **Jobs are plain Quartz `IJob` classes.** The scheduler runs each one inside `MonitoredJob<TJob>`, which gives each
  run:
  - one root span `Job <name>`
  - a log scope with `JobName` and `JobRunId`
  - one summary log line
  - the `acme.job.*` metrics (`runs`, `duration`, `active`, `last_success.time`, `misfires`), with the outcome
    (`success`, `idle`, `failed`, `cancelled`) and `error.type` as tags
- Jobs contain no business logic. Real work is a module command sent through `IMediator`, like a gRPC service method.
- Monitoring (dashboard and alert rules) lives in wst-cicd `otel`, next to the other WST dashboards.

## Consequences

- Prometheus gets `acme_job_*` series, low cardinality: one label set per job and outcome.
- Alerts can detect failed, stale, missing and misfired jobs.
- A scale-out of Core.Api would run every job once per instance. That then needs a persistent, clustered Quartz store
  or a separate worker host, and a new ADR.
- New metrics must follow `otel/METRICS.md`:
  - meters are named `Acme.*`
  - attributes are bounded
  - no ids, user data or messages as attributes
