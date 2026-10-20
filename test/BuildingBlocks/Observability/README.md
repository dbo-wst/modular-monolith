# BuildingBlocks Observability Tests

Unit tests for `src/BuildingBlocks/Observability`: resource attributes and whether tracing is registered. Assembly:
`Acme.Core.BuildingBlocks.Observability.Test`.

## Contents

| Item | Kind | Description |
|---|---|---|
| `ObservabilityExtensionsTests.cs` | test class | Resource attributes from `OTEL_RESOURCE_ATTRIBUTES`, `unknown` environment default, `service.instance.id` = machine name, `service.version` = entry assembly version without `+<commit>`, both overridden by `OTEL_RESOURCE_ATTRIBUTES`, no `TracerProvider`/`MeterProvider` without endpoint, one each with it |
| `Acme.Core.BuildingBlocks.Observability.Test.csproj` | project | Test project definition |

## How it works

Each test builds an in-memory `IConfiguration` with the `OTEL_*` keys and calls `ResourceAttributes` or
`AddObservability` on a real `ServiceCollection`. Nothing is exported: no span is started. The host integration tests
(`test_integration/Host`) cover the hosts starting without telemetry configuration.

## Running

```powershell
dotnet test .\test\BuildingBlocks\Observability\Acme.Core.BuildingBlocks.Observability.Test.csproj
```

## Dependencies

- References: `src/BuildingBlocks/Observability/Acme.Core.BuildingBlocks.Observability.csproj`
- Packages: `xunit.v3`, `xunit.runner.visualstudio`, `Microsoft.NET.Test.Sdk`, `Microsoft.Extensions.Configuration`,
  `Microsoft.Extensions.DependencyInjection`
