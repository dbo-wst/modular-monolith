# BuildingBlocks Tests

Unit tests for the shared BuildingBlocks layers in `src/BuildingBlocks`. One test project per layer; there is currently no unit test project for BuildingBlocks Persistence (it is exercised by `test/Modules/Ordering` and `test_integration/Modules/Ordering/Persistence`).

## Contents

| Item | Kind | Description |
|---|---|---|
| [`Domain`](Domain/README.md) | folder (test project) | Errors, domain events/aggregate roots and functional `Fin`/`Option` extensions |
| [`Application`](Application/README.md) | folder (test project) | Mediator, pipeline behaviours, domain event dispatcher and DI registration |

## How it works

Both projects are pure in-memory tests with no infrastructure. They reference `test/TestCompanion` for shared helpers.

## Running

```powershell
dotnet test .\test\BuildingBlocks\Domain\Template.Core.BuildingBlocks.Domain.Test.csproj
dotnet test .\test\BuildingBlocks\Application\Template.Core.BuildingBlocks.Application.Test.csproj
```

## Dependencies

- References: `src/BuildingBlocks/Domain`, `src/BuildingBlocks/Application`, `test/TestCompanion`

## Extending

Add a test project per BuildingBlocks layer at `test/BuildingBlocks/<Layer>/Template.Core.BuildingBlocks.<Layer>.Test.csproj`, mirror the `src` folder structure inside it and register it in `Template.Core.slnx`.
