# Template.Core.Api Integration Tests

gRPC-level tests of the core host `Template.Core.Api`: the Ordering `OrderService` and `OrderItemService` end to end through mediator and handlers, with persistence faked. Assembly: `Template.Core.Api.IntegrationTest`.

## Contents

| Item | Kind | Description |
|---|---|---|
| `CoreApiFactory.cs` | fixture | `WebApplicationFactory<Program>` in environment `IntegrationTest`; loads this project's `appsettings.json`, fakes `IUnitOfWork`, `IOrderRepository`, `IOrderItemRepository`, `IOrderingReadStore`; creates gRPC clients over the test server |
| `TestsCollectionDefinition.cs` | collection | xUnit collection `Template.Core.Api.IntegrationTest` with parallelization disabled |
| `Ordering/Order/OrderServiceTests.cs` | test class | Create/get/list/update/delete orders, validation and not-found errors, partial updates, error code classified by error type rather than message |
| `Ordering/OrderItem/OrderItemServiceTests.cs` | test class | Create/get/list/update/delete order items, validation error on missing fields |
| `appsettings.json` | config | Placeholder `Persistence:ConnectionString` so the host's options validation passes on start |
| `Template.Core.Api.IntegrationTest.csproj` | project | Test project definition (`Microsoft.NET.Sdk.Web`) |

## How it works

The host's content root is `src/Host/Template.Core.Api`, so `CoreApiFactory` explicitly adds `appsettings.json` from `AppContext.BaseDirectory`. The connection string there is never used: all stores are FakeItEasy fakes exposed as factory properties (`OrderingUnitOfWork`, `OrderRepository`, `OrderItemRepository`, `OrderingReadStore`) for arranging and verifying calls. Clients are created with `CreateOrderingOrderClient()` / `CreateOrderingOrderItemClient()`, which use a `GrpcChannel` over `Server.CreateHandler()`. No Docker or SQL Server is needed.

## Running

```powershell
dotnet test .\test_integration\Host\Template.Core.Api.IntegrationTest\Template.Core.Api.IntegrationTest.csproj
```

## Dependencies

- References: `src/Host/Template.Core.Api/Template.Core.Api.csproj`, `src/Protocol/Template.Core.Protocol.csproj`, `src/Modules/Ordering/Domain/Template.Core.Ordering.Domain.csproj`
- Packages: `FakeItEasy`, `Grpc.Net.Client`, `Microsoft.AspNetCore.Mvc.Testing`, `xunit.v3`, `xunit.runner.visualstudio`, `Microsoft.NET.Test.Sdk`

## Extending

For a new module, reference its Domain/Application projects, add fake properties for its repositories/read store and swap them in `ConfigureWebHost`, add a `Create<Module><Service>Client()` method, and put tests in `<Module>/<Aggregate>/<Service>Tests.cs` in collection `TestsCollectionDefinition.CollectionName`.
