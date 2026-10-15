# Ordering Application Integration Tests

Runs the Ordering vertical-slice handlers against a real SQL Server database to verify they work with the production EF Core mapping and migrations. Assembly: `Template.Core.Ordering.Application.IntegrationTest`.

## Contents

| Item | Kind | Description |
|---|---|---|
| `TestsCollectionDefinition.cs` | collection | Binds collection `IntegrationTestsFixture.CollectionName` to `IntegrationTestsFixture` |
| `Orders/CreateOrder/CreateOrderHandlerTests.cs` | test class | Create then get round-trip with `New` status, missing required fields fail |
| `Orders/GetOrder/GetOrderHandlerTests.cs` | test class | Missing order returns fail |
| `Orders/GetListOrder/GetListOrderHandlerTests.cs` | test class | Paged list contains created orders, page beyond data is empty |
| `Orders/UpdateOrder/UpdateOrderHandlerTests.cs` | test class | Full and partial updates (number, effective date, status), all-null update preserves entity, missing order fails |
| `Orders/DeleteOrder/DeleteOrderHandlerTests.cs` | test class | Delete existing order, missing order fails |
| `OrderItems/CreateOrderItem/CreateOrderItemHandlerTests.cs` | test class | Create then get order item |
| `OrderItems/GetOrderItem/GetOrderItemHandlerTests.cs` | test class | Missing order item returns fail |
| `OrderItems/GetListOrderItem/GetListOrderItemHandlerTests.cs` | test class | List returns created order items |
| `OrderItems/UpdateOrderItem/UpdateOrderItemHandlerTests.cs` | test class | Update then get order item |
| `OrderItems/DeleteOrderItem/DeleteOrderItemHandlerTests.cs` | test class | Delete existing order item, missing order item fails |
| `appsettings.json` | config | Logging/Serilog settings (console and rolling file `logs/log-.log`) |
| `Template.Core.Ordering.Application.IntegrationTest.csproj` | project | Test project definition |

## How it works

Test classes are in `[Collection(IntegrationTestsFixture.CollectionName)]` and receive `IntegrationTestsFixture`. Command handlers are built with `OrderingPersistence.Create(fixture.GetRequiredService<IOrderingDbContextFactory>()).ForOrders(...)` / `ForOrderItems(...)`; query handlers use `new ReadStore(factory)`. The database is shared by all tests in the run, so assertions are written against the entities a test created, not absolute counts. Requires Docker (default) or a SQL Server configured via `appsettings.test.json`; see [`test_integration/readme.md`](../../../readme.md).

## Running

```powershell
dotnet test .\test_integration\Modules\Ordering\Application\Template.Core.Ordering.Application.IntegrationTest.csproj
```

## Dependencies

- References: `src/Modules/Ordering/Application`, `src/Modules/Ordering/Domain`, `src/Modules/Ordering/Persistence`, `test_integration/Modules/Ordering/TestCompanion/Template.Core.Ordering.TestCompanion.csproj`
- Packages: `xunit.v3`, `xunit.runner.visualstudio`, `Microsoft.NET.Test.Sdk`, `Serilog`, `Serilog.Extensions.Logging`, `Serilog.Settings.Configuration`

## Extending

For a new slice add `<Aggregate>s/<Slice>/<Slice>HandlerTests.cs` with `[Collection(IntegrationTestsFixture.CollectionName)]`, take `IntegrationTestsFixture` in the primary constructor and build the handler through `OrderingPersistence` or `ReadStore`. Avoid assertions that depend on the total number of rows.
