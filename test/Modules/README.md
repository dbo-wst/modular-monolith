# Module Tests

Unit tests per business module. Each module folder mirrors `src/Modules/<Module>` with one test project per layer plus a module-specific test companion.

## Contents

| Item | Kind | Description |
|---|---|---|
| [`Ordering`](Ordering/README.md) | folder | Unit tests for the Ordering module (Domain, Application) and its test companion |

## How it works

Module tests run fully in-process. Persistence is replaced by the EF Core in-memory provider through the module's `TestCompanion`, so no Docker or SQL Server is needed.

## Running

```powershell
dotnet test .\test\Modules\Ordering\Domain\Template.Core.Ordering.Domain.Test.csproj
dotnet test .\test\Modules\Ordering\Application\Template.Core.Ordering.Application.Test.csproj
```

## Dependencies

- References: `src/Modules/<Module>/*`, `test/TestCompanion`

## Extending

For a new module `<Module>`, create `test/Modules/<Module>/` with `Domain/Template.Core.<Module>.Domain.Test.csproj`, `Application/Template.Core.<Module>.Application.Test.csproj` and `TestCompanion/Template.Core.<Module>.TestCompanion.csproj`, following the Ordering layout, and register the projects in `Template.Core.slnx`.
