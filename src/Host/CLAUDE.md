# CLAUDE.md — Host

Composition roots and transport adapters. **No business logic here.** Hosts translate between transport messages
and module Contracts, call `IMediator`, and translate results and errors back.

## Projects

- `Acme.Core.Api`: the gRPC server. It is the only place that composes modules:
  - `Program.cs` calls `AddMediator()` once, then `Add<Module>Module(configuration)` per module, then `Map<Module>GrpcServices()`.
  - Per module, `Prelaunch/<Module>/` holds `ServiceCollectionExtensions`, and each aggregate has its own folder with
    `GrpcService.cs`, `ProtoMappers.cs` and `Mappers/<Operation>/`.
- `Acme.Api`: the HTTP gateway (minimal APIs).
  - Routes are `/<segment>/<resource>`, for example `/ordering/orders` and `/ordering/order-items`.
  - It calls Core.Api through the client abstractions in `Acme.Api.Infrastructure.CoreApi`.
  - It must not reference module Domain, Application or Persistence projects.
- `Acme.Api.Infrastructure.CoreApi`: the gateway's gRPC client adapter. It holds named gRPC clients, the request and
  response models, and the proto mappers.

## Rules

- A gRPC service method must:
  1. Map the proto request to a Contracts command or query.
  2. Call `mediator.SendAsync(..., context.CancellationToken)`.
  3. `Match` the result into the success branch or the error branch of the response `oneof`.
- Mapping lives in explicit mapper classes (`IOnewayMap`) under `Mappers/<Operation>/`, never inline in services or endpoints.
- Error translation is central:
  - `ErrorCodeClassifier` picks the code from the `AppError` type, then the first `AppError` inside `ManyErrors`, then
    the inner error, and otherwise returns `internal`.
  - The gateway maps codes to HTTP: `not_found` 404, `validation` 400, `conflict`/`cancelled` 409, anything else 500.
  - Never classify by message text, and never leak exception details to clients.
- Configuration:
  - Use options classes with FluentValidation and `ValidateOnStart`: `Persistence:ConnectionString` for Core.Api,
    `CoreApi:BaseUrl` for the gateway.
  - Secrets come only from environment variables or local appsettings files, never from committed files.
- Logging goes through Serilog configuration. Do not use `Console.Write`. File sink paths start with `%BASEDIR%`
  (set to `AppContext.BaseDirectory` first thing in `Program.cs`), so files go to the execution folder, never the
  working directory.
- Telemetry: every host calls `WriteToOtlp(configuration, ServiceName)` on its Serilog configuration and
  `AddObservability(configuration, ServiceName)` (`src/BuildingBlocks/Observability`), `ServiceName` = `acme-<app>`.
  Logs, traces, `Acme.*` and runtime metrics, exported when `OTEL_EXPORTER_OTLP_ENDPOINT` is set (ADR 0002). Name
  custom `ActivitySource`s and `Meter`s `Acme.*`; keep metric attributes bounded (no ids, no user data). Job telemetry
  uses the shared `scheduled_job.*` names (wst-cicd `otel/TELEMETRY.md`); never enable Quartz's own tracing.
- New transport operation: add the proto first (`src/Protocol`), then the Core.Api service method plus mappers,
  then the gateway client plus endpoint, then integration tests in `test_integration/Host`.
- Background jobs (Core.Api `Jobs/`, Quartz): plain `IJob` classes (the scheduler wraps them in
  `MonitoredJob<TJob>` for telemetry; report idle/failure via `context.Result`), schedule via `JobsOptions`, call `IMediator`
  for any real work; no business logic in jobs. See `doc/08-cross-cutting-concepts/11-background-jobs.md`.
- Leave `KnownProxies`/`KnownIPNetworks` hardening (an open TODO) explicitly unresolved rather than widening it further.
