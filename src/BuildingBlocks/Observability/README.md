# Observability

OTLP export of logs, traces and metrics for the hosts (namespace `Acme.Core.BuildingBlocks.Observability`, project
`Acme.Core.BuildingBlocks.Observability`). The receiver is the `otel` stack of wst-cicd (`otel/DOTNET.md`). Only
Acme's own meters (`Acme.*`) are exported as metrics (ADR 0002).

## Contents

| Item | Kind | Description |
|---|---|---|
| `ObservabilityExtensions.cs` | static class | `AddObservability` (traces, metrics), `WriteToOtlp` (Serilog logs), `ResourceAttributes`, the `OTEL_*` key constants, `AcmeInstrumentation` (`Acme.*`) |
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
  OTEL_EXPORTER_OTLP_ENDPOINT=https://otel-local.steubing.com:4318
  OTEL_EXPORTER_OTLP_PROTOCOL=http/protobuf
  OTEL_RESOURCE_ATTRIBUTES=deployment.environment.name=local
  ```

- Resource attributes, identical on all signals: `service.name`, `service.namespace` = `acme`, plus the pairs of
  `OTEL_RESOURCE_ATTRIBUTES`; `deployment.environment.name` is `unknown` when not declared.
- Traces: ASP.NET Core requests (REST and gRPC server), outgoing HTTP calls (the gateway's gRPC clients, which carry
  the trace context to Core.Api) and SQL Server commands. The OTLP exporter reads endpoint and protocol from
  configuration and appends `/v1/traces`. Spans of every `ActivitySource` named `Acme.*` (e.g. `Acme.Core.Api.Jobs`)
  are included.
- Metrics: every `Meter` named `Acme.*` (e.g. `Acme.Core.Api.Jobs`, see
  `doc/08-cross-cutting-concepts/11-background-jobs.md`), exported over OTLP (`/v1/metrics`) to Prometheus.
  Exemplars are trace-based, so a histogram bucket links to the trace that recorded it. Runtime, ASP.NET Core and HTTP
  metrics are not exported; request rates come from Tempo span metrics.
- Logs: a Serilog sink next to console and files, Information and above, endpoint `<endpoint>/v1/logs` for
  `http/protobuf`. Every event carries the current trace and span id, so Grafana links a log line to its trace. The
  sink's own export requests are not traced; export failures go to stderr (`SelfLog`).
- The protocol defaults to gRPC when `OTEL_EXPORTER_OTLP_PROTOCOL` is unset, as in the SDK; with a `:4318` endpoint
  always set `http/protobuf`.

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
- Further meters (runtime, ASP.NET Core) would be one `AddMeter` here, but need a decision first (ADR 0002).
