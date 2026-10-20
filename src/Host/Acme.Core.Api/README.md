# Acme.Core.Api

gRPC host of the modular monolith (assembly `Acme.Core.Api`, `Microsoft.NET.Sdk.Web`). It composes the modules (Application + Persistence), registers the single shared mediator, exposes the module gRPC services defined in `Acme.Core.Protocol` and runs the Quartz background jobs.

## Contents

| Item | Kind | Description |
|---|---|---|
| `Program.cs` | entry point | Serilog bootstrap (plus `WriteToOtlp`), `AddObservability`, `AddGrpc`, `AddMediator` (once), `AddOrderingModule`, `AddBackgroundJobs`, `MapOrderingGrpcServices`, plain-text `GET /`. |
| [`Jobs/`](Jobs/README.md) | folder | Quartz background jobs (`Heartbeat`, `Housekeeping`, every 5 minutes) as plain `IJob` classes, run inside `MonitoredJob<TJob>`: root span, log scope and `scheduled_job.*` metrics per run. |
| [`Prelaunch/`](Prelaunch/README.md) | folder | `ErrorCodeClassifier` and per-module gRPC services and mappers (currently `Ordering`). |
| `appsettings.json` | config | Serilog (console + files with `[{TraceId}]` under `%BASEDIR%logs/`, i.e. the execution folder), `AllowedHosts`, `Kestrel:EndpointDefaults:Protocols = Http2`, `Jobs` (scheduler switch and cron per job), `Quartz` logs at Warning. |
| `appsettings.Local.json` | config | `Persistence:ConnectionString` for local SQL Server (`acme-local`). |
| `Acme.Core.Api.csproj` | project | Project file. |

## How it works

DI composition in `Program.cs`:

```csharp
builder.Services.AddGrpc();
builder.Services.AddMediator();                       // one IMediator + pipeline behaviours + domain event dispatcher for all modules
builder.Services.AddOrderingModule(builder.Configuration); // AddOrderingApplication() + AddOrderingPersistence(configuration)
builder.Services.AddBackgroundJobs(builder.Configuration); // Quartz scheduler + JobSchedulerHostedService (section Jobs)
...
app.UseSerilogRequestLogging();
app.MapOrderingGrpcServices();                        // Order.GrpcService, OrderItem.GrpcService
```

- `AddMediator` (BuildingBlocks.Application) registers `IMediator`, `IDomainEventDispatcher` and `ValidationBehavior`; modules only register their handlers/validators.
- `AddOrderingPersistence` binds section `Persistence` (`Persistence:ConnectionString`, validated non-empty on start) and configures SQL Server.
- Telemetry: service `acme-core-api`; with `OTEL_EXPORTER_OTLP_*` set it exports logs (Information+), traces (gRPC requests, SQL commands, job runs), the `scheduled_job.*` and runtime metrics over OTLP, otherwise nothing. The start line logs the OTLP endpoint or `off`.
- Kestrel serves HTTP/2 only; locally at `http://localhost:6001` (`launchSettings.json`, environment `Local`).
- `GET /` returns a hint that gRPC endpoints require a gRPC client.
- `public partial class Program` in namespace `Acme.Core.Api` supports `WebApplicationFactory` tests.
- Commented-out references/usings for `Allocation`, `ClientSide`, `MasterData` show where further modules plug in.

## Dependencies
- References: `Acme.Core.Protocol`, `Acme.Core.BuildingBlocks.Observability`, `Acme.Core.Ordering.Application`, `Acme.Core.Ordering.Persistence`; packages `Grpc.AspNetCore`, `Quartz.Extensions.Hosting`, `Serilog`, `Serilog.AspNetCore`, `Serilog.Settings.Configuration`, `Serilog.Sinks.Console`, `Serilog.Sinks.File`.
- Used by: `Acme.Api` (via gRPC clients in `Acme.Api.Infrastructure.CoreApi`); `test/Host/Acme.Core.Api` (job unit tests, via `InternalsVisibleTo`); `test_integration/Host/Acme.Core.Api.IntegrationTest`.

## Extending
New module: reference its `Application` and `Persistence` projects, add `Prelaunch/<Module>/ServiceCollectionExtensions.cs` with `Add<Module>Module` and `Map<Module>GrpcServices`, then call both in `Program.cs`. Do not call `AddMediator` from modules. New job: see [`Jobs/`](Jobs/README.md).
