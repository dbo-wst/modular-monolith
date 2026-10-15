# Ordering Application Tests

Unit tests for the Ordering application layer: every vertical-slice handler, the mediator pipeline with validation, and domain event dispatch through the unit of work. Runs against the EF Core in-memory provider. Assembly: `Template.Core.Ordering.Application.Test`.

## Contents

| Item | Kind | Description |
|---|---|---|
| `MediatorPipelineTests.cs` | test class | Full DI wiring (`AddMediator` + `AddOrderingApplication`): invalid commands fail with `ValidationError` before the handler, valid commands are handled |
| `DomainEvents/DomainEventDispatchTests.cs` | test class | Events dispatched only after a successful save and then cleared, events of deleted aggregates dispatched, nothing dispatched on concurrency conflict, dispatch failure returned, `OrderCreated` reaching a registered handler via the mediator |
| `Orders/CreateOrder/CreateOrderHandlerTests.cs` | test class | Create then get round-trip, server-side id and `New` status, missing required fields fail |
| `Orders/GetOrder/GetOrderHandlerTests.cs` | test class | Get existing order, missing order fails |
| `Orders/GetListOrder/GetListOrderHandlerTests.cs` | test class | List, empty list, page size, page beyond data (isolated database per test) |
| `Orders/UpdateOrder/UpdateOrderHandlerTests.cs` | test class | Full and partial updates, no-op updates, missing order, valid/invalid status transitions |
| `Orders/DeleteOrder/DeleteOrderHandlerTests.cs` | test class | Delete existing order, missing order fails |
| `OrderItems/CreateOrderItem/CreateOrderItemHandlerTests.cs` | test class | Create then get, missing/invalid quantity and price fail |
| `OrderItems/GetOrderItem/GetOrderItemHandlerTests.cs` | test class | Get created order item |
| `OrderItems/GetListOrderItem/GetListOrderItemHandlerTests.cs` | test class | List returns created order items |
| `OrderItems/UpdateOrderItem/UpdateOrderItemHandlerTests.cs` | test class | Update then get, missing order item fails |
| `OrderItems/DeleteOrderItem/DeleteOrderItemHandlerTests.cs` | test class | Delete existing order item, missing order item fails |
| `Template.Core.Ordering.Application.Test.csproj` | project | Test project definition |

## How it works

- Handler tests use `IClassFixture<InMemoryFixture>`; the fixture's `ContextFactory` is a FakeItEasy `IOrderingDbContextFactory` over a fresh in-memory database (`Support.Infrastructure.DbContextFactory()`).
- Command handlers are built with `OrderingPersistence.Create(factory).ForOrders((r, u) => new CreateOrderHandler(r, u))` (or `ForOrderItems`), so repositories and `UnitOfWork` share one `OrderingDbContext` like one DI scope. Query handlers get `new ReadStore(factory)`.
- Count-sensitive list tests create their own factory per test instead of using the shared fixture.
- `DomainEventDispatchTests` passes a `RecordingDomainEventDispatcher` (optionally failing) to `OrderingPersistence.Create` and inspects `Dispatched`.
- `MediatorPipelineTests` and the last `DomainEventDispatchTests` case build a real `ServiceCollection` with the production registrations and the in-memory `DbContext`.

No Docker or SQL Server is required.

## Running

```powershell
dotnet test .\test\Modules\Ordering\Application\Template.Core.Ordering.Application.Test.csproj
```

## Dependencies

- References: `src/Modules/Ordering/Application/Template.Core.Ordering.Application.csproj`, `src/Modules/Ordering/Domain/Template.Core.Ordering.Domain.csproj`, `test/Modules/Ordering/TestCompanion/Template.Core.Ordering.TestCompanion.csproj`
- Packages: `xunit.v3`, `xunit.runner.visualstudio`, `Microsoft.NET.Test.Sdk`, `Microsoft.Extensions.DependencyInjection`

## Extending

For a new slice add `<Aggregate>s/<Slice>/<Slice>HandlerTests.cs` (mirroring `src/Modules/Ordering/Application`), take `InMemoryFixture` via `IClassFixture`, build command handlers through `OrderingPersistence.Create(...).ForOrders/ForOrderItems(...)` and query handlers with `ReadStore`. Add validation cases for the slice's validator to `MediatorPipelineTests`.
