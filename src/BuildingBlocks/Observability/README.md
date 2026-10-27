# Observability

OTLP export of logs, traces and metrics for the hosts (namespace `Acme.Core.BuildingBlocks.Observability`, project
`Acme.Core.BuildingBlocks.Observability`). The receiver is the `otel` stack of wst-cicd (`otel/DOTNET.md`). Exported
metrics are Acme's own meters (`Acme.*`) and the .NET runtime meter `System.Runtime` (ADR 0002).

## Contents

| Item | Kind | Description |
|---|---|---|
| `ObservabilityExtensions.cs` | static class | `AddObservability` (traces, metrics), `WriteToOtlp` (Serilog logs), `ResourceAttributes`, the `OTEL_*` key and resource attribute constants, `AcmeInstrumentation` (`Acme.*`), `RuntimeInstrumentation` (`System.Runtime`) |
| `Acme.Core.BuildingBlocks.Observability.csproj` | project | Project file |

## How it works

Each host calls both with its service name (`acme-core-api`, `acme-api`):

```csharp
builder.Host.UseSerilog((context, services, loggerConfiguration) => loggerConfiguration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .WriteToOtlp(context.Configuration, ServiceName));

builder.Services.AddObservability(builder.Configuration, ServiceName);
```

- Both do nothing unless `OTEL_EXPORTER_OTLP_ENDPOINT` is set; without it the host runs unchanged.
- Configuration (environment variables, see `operations/cd/<app>/conf.d/`):

  ```
  OTEL_EXPORTER_OTLP_ENDPOINT=https://otel-local.steubing.com:4317
  OTEL_EXPORTER_OTLP_PROTOCOL=grpc
  OTEL_RESOURCE_ATTRIBUTES=deployment.environment.name=local
  ```

- Resource attributes, identical on all signals: `service.name`, `service.namespace` = `acme`,
  `service.version` = the entry assembly's informational version cut at `+` (the image version from
  `dotnet publish -p:Version`, `0.0.0` without one), `service.instance.id` = `Environment.MachineName` (the
  container's host name, which the compose files set to the container name), then the pairs of
  `OTEL_RESOURCE_ATTRIBUTES`, which override all of these; `deployment.environment.name` is `unknown` when not
  declared.
- Traces: ASP.NET Core requests (REST and gRPC server), outgoing HTTP calls (the gateway's gRPC clients, which carry
  the trace context to Core.Api) and SQL Server commands. The OTLP exporter reads endpoint and protocol from
  configuration (with `grpc` it uses the endpoint as is; with `http/protobuf` it appends `/v1/traces`). Spans of every `ActivitySource` named `Acme.*` (e.g. `Acme.Core.Api.Jobs`)
  are included.
- Metrics: every `Meter` named `Acme.*` (e.g. `Acme.Core.Api.Jobs` with the shared `scheduled_job.*` instruments,
  see `doc/08-cross-cutting-concepts/11-background-jobs.md`) and the runtime meter `System.Runtime` (GC, heap, thread
  pool, lock contention, exceptions, CPU), exported over OTLP to Prometheus. Exemplars are
  trace-based, so a histogram bucket links to the trace that recorded it. ASP.NET Core and HTTP metrics are not
  exported; request rates come from Tempo span metrics.
- Logs: a Serilog sink next to console and files, Information and above, over the same protocol: the endpoint as
  is for `grpc`, `<endpoint>/v1/logs` for `http/protobuf`. Every event carries the current trace and span id, so Grafana links a log line to its trace. Log
  scope entries become log record attributes under their key, dots included (e.g. `scheduled_job.name`). The
  sink's own export requests are not traced; export failures go to stderr (`SelfLog`).
- The standard is OTLP over gRPC on port 4317 (`grpc`). Always set `OTEL_EXPORTER_OTLP_PROTOCOL`: unset means gRPC
  in the SDK and in `WriteToOtlp`, which fails against the HTTP port `:4318`; that port needs `http/protobuf`.

## Dependencies
- References: OpenTelemetry (`Extensions.Hosting`, `Exporter.OpenTelemetryProtocol`, `Instrumentation.AspNetCore`,
  `.Http`, `.SqlClient`), `Serilog`, `Serilog.Sinks.OpenTelemetry`, configuration and DI abstractions. No project
  references.
- Used by: `src/Host/Acme.Core.Api`, `src/Host/Acme.Api`; tests in `test/BuildingBlocks/Observability`.

## Extending
- A new host: pick a service name `acme-<app>`, call both methods as above and add the three `OTEL_*` keys to its env
  files.
- Custom spans and metrics: create an `ActivitySource` / a `Meter` (via `IMeterFactory`) named `Acme.*` in the project
  that records; nothing to register here. Follow wst-cicd `otel/METRICS.md` (units, bounded attributes).
- Further meters (ASP.NET Core, HTTP) would be one `AddMeter` here, but need a decision first (ADR 0002).
