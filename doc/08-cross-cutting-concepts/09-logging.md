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

Both hosts export **logs and traces** over OTLP to the `otel` stack of wst-cicd; **metrics are not exported**. The
code lives in `src/BuildingBlocks/Observability` and is called once per host:

- `WriteToOtlp(configuration, ServiceName)` adds a Serilog OTLP sink (Information and above; Debug stays local). Code
  keeps logging through `ILogger<T>`; each log event carries the current trace and span id.
- `AddObservability(configuration, ServiceName)` traces ASP.NET Core requests, outgoing HTTP calls (the gateway's gRPC
  clients carry the trace context to Core.Api) and SQL Server commands.

| Resource attribute | Value |
|---|---|
| `service.name` | `acme-api` (gateway), `acme-core-api` (gRPC host) |
| `service.namespace` | `acme` |
| `deployment.environment.name` | from `OTEL_RESOURCE_ATTRIBUTES`, `unknown` when missing |

Export is on only when `OTEL_EXPORTER_OTLP_ENDPOINT` is set (environment, `operations/cd/<app>/conf.d/.env`); without
it the hosts run unchanged and tests export nothing:

```
OTEL_EXPORTER_OTLP_ENDPOINT=https://otel-local.steubing.com:4318
OTEL_EXPORTER_OTLP_PROTOCOL=http/protobuf
OTEL_RESOURCE_ATTRIBUTES=deployment.environment.name=local
```

Never put secrets, request bodies or personal data into log messages or span attributes.
