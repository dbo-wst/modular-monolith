# Ordering Domain Tests

Unit tests for the Ordering domain model: the `Order` and `OrderItem` aggregates, their invariants, status transitions, raised domain events and strongly typed ids. Assembly: `Template.Core.Ordering.Domain.Test`.

## Contents

| Item | Kind | Description |
|---|---|---|
| `Order/OrderTests.cs` | test class | `Order.Create` validation and `OrderCreated` event, permissive `Rehydrate`, `ChangeDetails` partial updates and `OrderUpdated`, valid/invalid status transitions, `Delete` raising `OrderDeleted` |
| `Order/OrderIdTests.cs` | test class | `OrderId.From`/`MaybeCreate`, nullable overload, value equality |
| `OrderItem/OrderItemTests.cs` | test class | `OrderItem.Create` validation (quantity > 0, price >= 0, order id required) and events, `Rehydrate`, `ChangeDetails`, `Delete` |
| `OrderItem/OrderItemIdTests.cs` | test class | `OrderItemId.From`/`MaybeCreate`, nullable overload, value equality |
| `Template.Core.Ordering.Domain.Test.csproj` | project | Test project definition |

## How it works

Pure unit tests: aggregates are built with `Create(...)` (validating, raises events) or `Rehydrate(...)` (permissive, no events) and asserted through their state and `DomainEvents`. Transition tests use `[Theory]` data over `OrderStatus` values. No fixtures or infrastructure.

## Running

```powershell
dotnet test .\test\Modules\Ordering\Domain\Template.Core.Ordering.Domain.Test.csproj
```

## Dependencies

- References: `src/Modules/Ordering/Domain/Template.Core.Ordering.Domain.csproj`, `test/Modules/Ordering/TestCompanion/Template.Core.Ordering.TestCompanion.csproj`
- Packages: `xunit.v3`, `xunit.runner.visualstudio`, `Microsoft.NET.Test.Sdk`

## Extending

For a new aggregate or value object add a folder named after it (e.g. `Shipment/`) with `<Aggregate>Tests.cs` and `<Aggregate>IdTests.cs`, covering `Create` validation, `Rehydrate`, every behaviour method and the domain events each one raises.
