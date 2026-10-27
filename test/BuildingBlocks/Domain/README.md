# BuildingBlocks Domain Tests

Unit tests for `src/BuildingBlocks/Domain`: typed errors, domain events and aggregate roots, and the LanguageExt-based functional helpers. Assembly: `Acme.Core.BuildingBlocks.Domain.Test`.

## Contents

| Item | Kind | Description |
|---|---|---|
| `Errors/ErrorCodesTests.cs` | test class | `ErrorCodes.Of`: typed error, first typed member of `ManyErrors`, inner typed error, untyped and exceptional errors as `internal` |
| `Errors/AppErrorTests.cs` | test class | Stable wire error codes, error type to code mapping, `ConcurrencyConflict`/`OperationCancelled` classification, `ToFin` on null |
| `Events/DomainEventTests.cs` | test class | Event id/UTC timestamp metadata, equality semantics, `AggregateRoot` raising and clearing events |
| `Functional/AdvancedTests.cs` | test class | Partial application (`Applying`, arity 1-5) and currying |
| `Functional/FinExtensionsTests.cs` | test class | Synchronous `Fin` helpers: `ToUnit`, `MapError`, `Try` |
| `Functional/FinAsyncExtensionsTests.cs` | test class | `Task<Fin<T>>` helpers: `ToUnitAsync`, `TryAsync`, `MapAsync`, `BindAsync`, `BindForceAsync`, `MatchAsync` |
| `Functional/FuncFinAsyncExtensionsTests.cs` | test class | The same async helpers applied to `Func<Task<Fin<T>>>` |
| `Functional/GeneralTests.cs` | test class | `ToError`, `Then`, `Unit.Success/Failure`, `ToFin`/`ToFinAsync` for values, `Option` and `Task<Option>` |
| `Acme.Core.BuildingBlocks.Domain.Test.csproj` | project | Test project definition |

## How it works

Plain xUnit v3 facts/theories against the domain building blocks; test types (aggregates, events) are declared inline as nested classes. No fixtures, no fakes, no infrastructure.

## Running

```powershell
dotnet test .\test\BuildingBlocks\Domain\Acme.Core.BuildingBlocks.Domain.Test.csproj
```

## Dependencies

- References: `src/BuildingBlocks/Domain/Acme.Core.BuildingBlocks.Domain.csproj`, `test/TestCompanion/Acme.Core.TestCompanion.csproj`
- Packages: `xunit.v3`, `xunit.runner.visualstudio`, `Microsoft.NET.Test.Sdk`

## Extending

Place tests in the folder matching the `src/BuildingBlocks/Domain` namespace (`Errors`, `Events`, `Functional`, ...), one test class per production type or extension class.
