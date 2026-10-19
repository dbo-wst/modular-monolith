# Acme.Api

HTTP/REST gateway (assembly `Acme.Api`, `Microsoft.NET.Sdk.Web`). It exposes Minimal API endpoints with OpenAPI/Scalar and forwards every call to `Acme.Core.Api` through the typed gRPC clients of `Acme.Api.Infrastructure.CoreApi`. It contains no business logic.

## Contents

| Item | Kind | Description |
|---|---|---|
| `Program.cs` | entry point | Serilog bootstrap (plus `WriteToOtlp`), `AddObservability`, OpenAPI, JSON string enums, `AddCoreApiInfrastructure`, forwarded headers, optional `PathBase`, Scalar, `MapOrderingEndpoints`. |
| `Segments.cs` | class | Route segment constants (`Segments.Ordering = "ordering"`). |
| [`Ordering/`](Ordering/README.md) | folder | Endpoint groups for the Ordering module (`/ordering/orders`, `/ordering/order-items`). |
| `appsettings.json` | config | Serilog (console, `%BASEDIR%logs/log-`, `error-`, `debug-` in the execution folder; files with `[{TraceId}]`), `AllowedHosts`. |
| `appsettings.Local.json` | config | `CoreApi:BaseUrl = http://localhost:6001`. |
| `Acme.Api.csproj` | project | Project file. |

## How it works

`Program.cs` composition:

1. Bootstrap logger from `appsettings.json` + `appsettings.{ASPNETCORE_ENVIRONMENT|DOTNET_ENVIRONMENT}.json`, then `builder.Host.UseSerilog(...)` with `WriteToOtlp`, and `AddObservability` (service `acme-api`: logs and traces of HTTP requests and the outgoing gRPC calls, exported only when `OTEL_EXPORTER_OTLP_ENDPOINT` is set).
2. `AddOpenApi` with schema ids = full type name with the `Acme.Api.` prefix stripped (for `Acme.Api*` namespaces).
3. `ConfigureHttpJsonOptions` adds `JsonStringEnumConverter` (e.g. `EStatus` is serialized as `"IN_PROGRESS"`).
4. `AddCoreApiInfrastructure(configuration)` registers `CoreApiOptions` (validated `CoreApi:BaseUrl`) and the gRPC clients.
5. `ForwardedHeadersOptions`: `X-Forwarded-For/Proto/Host/Prefix`; `KnownProxies`/`KnownIPNetworks` are cleared (marked TODO to restrict).
6. Pipeline: `UseForwardedHeaders` -> `UseSerilogRequestLogging` -> `UsePathBase(PathBase)` if configured -> `MapOpenApi` -> `MapScalarApiReference` (title "Acme.Api Reference") -> `MapOrderingEndpoints`.

Configuration keys: `CoreApi:BaseUrl` (required, absolute URL), `PathBase` (optional), `Serilog`, `OTEL_EXPORTER_OTLP_ENDPOINT` / `OTEL_EXPORTER_OTLP_PROTOCOL` / `OTEL_RESOURCE_ATTRIBUTES` (optional).

`public partial class Program` is declared in namespace `Acme.Api` for `WebApplicationFactory` in integration tests.

## Dependencies
- References: `Acme.Api.Infrastructure.CoreApi`, `Acme.Core.BuildingBlocks.Observability`; packages `LanguageExt.Core`, `Microsoft.AspNetCore.OpenApi`, `Scalar.AspNetCore`, `Serilog`, `Serilog.AspNetCore`, `Serilog.Settings.Configuration`, `Serilog.Sinks.Console`, `Serilog.Sinks.File`.
- Used by: external HTTP clients; `test_integration/Host/Acme.Api.IntegrationTest`.

## Extending
- New module: add a `Segments` constant, a `<Module>/` folder with `Endpoints.cs` per resource and a `Map<Module>Endpoints(this WebApplication)` extension, then call it in `Program.cs` after `MapOrderingEndpoints()`.
- The matching gRPC clients must be registered in `Acme.Api.Infrastructure.CoreApi` (`AddCoreApiInfrastructure`).
