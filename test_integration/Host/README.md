# Host Integration Tests

In-process tests of the two ASP.NET Core hosts using `WebApplicationFactory<Program>` and FakeItEasy fakes for everything behind the host boundary. No Docker or SQL Server is required.

## Contents

| Item | Kind | Description |
|---|---|---|
| [`Acme.Api.IntegrationTest`](Acme.Api.IntegrationTest/README.md) | folder (test project) | HTTP gateway (`Acme.Api`) endpoints with faked gRPC clients |
| [`Acme.Core.Api.IntegrationTest`](Acme.Core.Api.IntegrationTest/README.md) | folder (test project) | gRPC services of `Acme.Core.Api` with faked repositories, unit of work and read store |

## How it works

Each project has its own factory (`GatewayApiFactory`, `CoreApiFactory`) that runs the host in the `IntegrationTest` environment and replaces infrastructure services with fakes. Test classes share a non-parallel xUnit collection (`TestsCollectionDefinition`, `DisableParallelization = true`) because the fakes are shared state.

## Running

```powershell
dotnet test .\test_integration\Host\Acme.Api.IntegrationTest\Acme.Api.IntegrationTest.csproj
dotnet test .\test_integration\Host\Acme.Core.Api.IntegrationTest\Acme.Core.Api.IntegrationTest.csproj
```

## Dependencies

- References: `src/Host/Acme.Api`, `src/Host/Acme.Api.Infrastructure.CoreApi`, `src/Host/Acme.Core.Api`, `src/Protocol`, `src/Modules/Ordering/Domain`

## Extending

When a module is exposed by a host, add a `<Module>/<Aggregate>/` folder in the matching project and register fakes for its services in the factory.
