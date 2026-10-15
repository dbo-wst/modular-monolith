# Ordering Module Tests

Unit tests for the Ordering module (`src/Modules/Ordering`): aggregates and value objects in Domain, and the vertical-slice handlers in Application.

## Contents

| Item | Kind | Description |
|---|---|---|
| [`Domain`](Domain/README.md) | folder (test project) | `Order`/`OrderItem` aggregates and their id value objects |
| [`Application`](Application/README.md) | folder (test project) | Command/query handlers, mediator pipeline and domain event dispatch |
| [`TestCompanion`](TestCompanion/README.md) | folder (library) | In-memory `IOrderingDbContextFactory`, `InMemoryFixture`, `OrderingPersistence`, `RecordingDomainEventDispatcher` |

## How it works

Domain tests need nothing but the domain assembly. Application tests wire real handlers, repositories and `UnitOfWork` against an EF Core in-memory database created by `TestCompanion`. No Docker or SQL Server is required; the SQL Server variants live in `test_integration/Modules/Ordering`.

## Running

```powershell
dotnet test .\test\Modules\Ordering\Domain\Acme.Core.Ordering.Domain.Test.csproj
dotnet test .\test\Modules\Ordering\Application\Acme.Core.Ordering.Application.Test.csproj
```

## Dependencies

- References: `src/Modules/Ordering/Domain`, `src/Modules/Ordering/Application`, `src/Modules/Ordering/Persistence` (via TestCompanion), `test/TestCompanion`

## Extending

- New aggregate: add `Domain/<Aggregate>/` tests and `Application/<Aggregate>s/<Slice>/` handler tests.
- New layer project (e.g. Contracts): add `test/Modules/Ordering/<Layer>/Acme.Core.Ordering.<Layer>.Test.csproj` and register it in `Acme.Core.slnx`.
