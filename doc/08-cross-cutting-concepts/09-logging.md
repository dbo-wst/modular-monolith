# 8.9 Logging

Serilog, configured per host in `appsettings.json`: console and rolling files under `logs/` in the execution folder
(`bin/<configuration>/net10.0/logs/` locally, `/app/publish/logs/` in the container; the File sink paths start with
`%BASEDIR%`, which `Program.cs` sets to `AppContext.BaseDirectory`); file lines carry the
trace id (`[{TraceId}]`).

## Failed requests

`FailureLoggingBehavior` (outermost mediator behaviour, `src/BuildingBlocks/Application/Communication`) logs every
command or query that returns a failed `Fin`, once, with the request type name and the error code from
`ErrorCodes.Of`:

| Code | Level | EventId | Message |
|---|---|---|---|
| `not_found`, `validation`, `conflict`, `cancelled` | Warning | 1001 `RequestRejected` | `{RequestName} rejected: {ErrorCode} {ErrorMessage}` |
| `internal` (persistence failures, untyped errors) | Error | 1002 `RequestFailed` | `{RequestName} failed: {ErrorCode} {ErrorMessage}`, with the exception |

For `RequestFailed` the current span is marked as failed (status Error, `error.type`). The lines go to the
console, the files and, when configured, OTLP. Handlers and hosts do not log the same failure again; exceptions
thrown by handlers are logged by ASP.NET Core / gRPC.

## Telemetry (OpenTelemetry)

Both hosts export **logs, traces and metrics** over OTLP to the `otel` stack of wst-cicd; the metrics are Acme's own
meters (`Acme.*`) and the .NET runtime meter (`System.Runtime`), not the ASP.NET Core or HTTP meters
([ADR 0002](../09-architecture-decisions/0002-metrics-and-background-jobs.md)). The code lives in
`src/BuildingBlocks/Observability` and is called once per host:

- `WriteToOtlp(configuration, ServiceName)` adds a Serilog OTLP sink (Information and above; Debug stays local). Code
  keeps logging through `ILogger<T>`; each log event carries the current trace and span id.
- `AddObservability(configuration, ServiceName)` traces ASP.NET Core requests, outgoing HTTP calls (the gateway's gRPC
  clients carry the trace context to Core.Api), SQL Server commands and every `ActivitySource` named `Acme.*`; it also
  exports every `Meter` named `Acme.*`, with trace-based exemplars, and the runtime meter `System.Runtime` (GC, heap,
  thread pool, exceptions, CPU). The only `Acme.*` meter today is the background jobs' `Acme.Core.Api.Jobs`, which
  follows the shared `scheduled_job.*` contract (see [8.11 Background Jobs](11-background-jobs.md)).
- Log scope entries become attributes of the OTLP log record under their key, so a scope with dotted keys (e.g.
  `scheduled_job.name`) gives log lines the same attribute names as spans and metrics; message template placeholders
  cannot contain dots.

| Resource attribute | Value |
|---|---|
| `service.name` | `acme-api` (gateway), `acme-core-api` (gRPC host) |
| `service.namespace` | `acme` |
| `service.version` | entry assembly's informational version without `+<commit>`: the image version (`build.sh <version>` passes it as `-p:Version`), `0.0.0` when unknown |
| `service.instance.id` | `Environment.MachineName`, i.e. the container's host name; the compose files set `hostname` to the container name (`acme-api`, `acme-core-api`), so a recreated container keeps its instance |
| `deployment.environment.name` | from `OTEL_RESOURCE_ATTRIBUTES`, `unknown` when missing |

Pairs declared in `OTEL_RESOURCE_ATTRIBUTES` override these values.

Export is on only when `OTEL_EXPORTER_OTLP_ENDPOINT` is set (environment, `operations/cd/<app>/conf.d/.env`); without
it the hosts run unchanged and tests export nothing:

```
OTEL_EXPORTER_OTLP_ENDPOINT=https://otel-local.steubing.com:4317
OTEL_EXPORTER_OTLP_PROTOCOL=grpc
OTEL_RESOURCE_ATTRIBUTES=deployment.environment.name=local
```

Never put secrets, request bodies or personal data into log messages, span attributes or metric attributes; metric
attributes must also be bounded (no ids).
