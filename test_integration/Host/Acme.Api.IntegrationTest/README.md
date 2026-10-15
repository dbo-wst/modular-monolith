# Acme.Api Integration Tests

HTTP-level tests of the gateway host `Acme.Api`: routing, status codes, ProblemDetails and request-to-gRPC mapping for the Ordering endpoints. Assembly: `Acme.Api.IntegrationTest`.

## Contents

| Item | Kind | Description |
|---|---|---|
| `GatewayApiFactory.cs` | fixture | `WebApplicationFactory<Program>` in environment `IntegrationTest`; replaces the Ordering `Order` and `OrderItem` `IGrpcApiClient` registrations with static FakeItEasy fakes |
| `TestsCollectionDefinition.cs` | collection | xUnit collection `Acme.Api.IntegrationTest` with parallelization disabled |
| `Ordering/Order/EndpointsTests.cs` | test class | `/{Segments.Ordering}/orders` POST/GET/list/PUT/DELETE: 201/200/204, 400 on validation, 404 ProblemDetails, 409 on conflict, default paging, full lifecycle round-trip |
| `Ordering/Order/RequestMappingTests.cs` | test class | Optional/missing JSON fields are forwarded to the gRPC client as nulls, only provided fields are forwarded |
| `Ordering/OrderItem/EndpointsTests.cs` | test class | Order item POST/GET/list/PUT/DELETE status codes and forwarded requests |
| `Ordering/OrderItem/RequestMappingTests.cs` | test class | Partial PUT/POST bodies map to nullable gRPC request fields |
| `appsettings.json` | config | `CoreApi:BaseUrl`; copied to the output |
| `Acme.Api.IntegrationTest.csproj` | project | Test project definition (`Microsoft.NET.Sdk.Web`) |

## How it works

Test classes use `IClassFixture<GatewayApiFactory>` and `factory.CreateClient()`. The gRPC client fakes are static singletons on `GatewayApiFactory.Ordering.<Aggregate>.GrpcApiClient`, so each test class constructor calls `Fake.ClearRecordedCalls(...)` and each test configures its own behaviour; the non-parallel collection prevents interference. JSON uses `JsonSerializerDefaults.Web` plus `JsonStringEnumConverter`, matching the gateway. No real Core API, Docker or database is used.

## Running

```powershell
dotnet test .\test_integration\Host\Acme.Api.IntegrationTest\Acme.Api.IntegrationTest.csproj
```

## Dependencies

- References: `src/Host/Acme.Api/Acme.Api.csproj`, `src/Host/Acme.Api.Infrastructure.CoreApi/Acme.Api.Infrastructure.CoreApi.csproj`
- Packages: `FakeItEasy`, `Microsoft.AspNetCore.Mvc.Testing`, `xunit.v3`, `xunit.runner.visualstudio`, `Microsoft.NET.Test.Sdk`

## Extending

For a new module/aggregate exposed by the gateway, add a static fake for its `IGrpcApiClient` to `GatewayApiFactory`, swap the registration in `ConfigureWebHost` (`RemoveAll` + `AddScoped`), and add `<Module>/<Aggregate>/EndpointsTests.cs` and `RequestMappingTests.cs` in collection `TestsCollectionDefinition.CollectionName`.
