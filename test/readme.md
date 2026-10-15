# Tests

Fast, in-process unit tests for the building blocks, the Ordering module and the architecture rules. Nothing under `test/` needs Docker or SQL Server; persistence is backed by the EF Core in-memory provider.

## Structure

| Item | Kind | Description |
|---|---|---|
| [`Architecture`](Architecture/README.md) | folder (test project) | NetArchTest dependency rules for the Ordering module, BuildingBlocks and the gateway |
| [`BuildingBlocks`](BuildingBlocks/README.md) | folder | Unit tests for the shared BuildingBlocks layers (Domain, Application) |
| [`Modules`](Modules/README.md) | folder | Unit tests per business module (currently Ordering) |
| [`TestCompanion`](TestCompanion/README.md) | folder (library) | Shared, module-agnostic test helpers (`Support`) |
| `Directory.Build.props` | build props | Imports the repository-wide `Directory.Build.props` and suppresses `xUnit1051` |

## Prerequisites

- .NET SDK 10

## Run Tests

Run from the repository root (PowerShell). Run one test project:

```powershell
dotnet test .\test\Modules\Ordering\Domain\Template.Core.Ordering.Domain.Test.csproj
```

Test projects under `test/`:

```powershell
dotnet test .\test\Architecture\Template.Core.Architecture.Test.csproj
dotnet test .\test\BuildingBlocks\Domain\Template.Core.BuildingBlocks.Domain.Test.csproj
dotnet test .\test\BuildingBlocks\Application\Template.Core.BuildingBlocks.Application.Test.csproj
dotnet test .\test\Modules\Ordering\Domain\Template.Core.Ordering.Domain.Test.csproj
dotnet test .\test\Modules\Ordering\Application\Template.Core.Ordering.Application.Test.csproj
```

Run everything in the solution (unit and integration tests; integration tests need Docker, see `test_integration/readme.md`):

```powershell
dotnet test .\Template.Core.slnx
```

## Notes

- Tests use xUnit v3 (`Xunit` is a global using); fakes use FakeItEasy, test data uses Bogus.
- The folder layout mirrors `src/`: `test/<Area>/<Layer>` tests `src/<Area>/<Layer>`.
- Shared helpers live in `test/TestCompanion/`; module-specific helpers live in `test/Modules/<Module>/TestCompanion/`.
- Integration tests (real SQL Server, hosted APIs) live in `test_integration/`.
