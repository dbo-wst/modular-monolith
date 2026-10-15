# Test Companion (unit, shared)

Module-agnostic helpers shared by the unit test projects under `test/`. Assembly: `Template.Core.TestCompanion` (library, contains no tests).

## Contents

| Item | Kind | Description |
|---|---|---|
| `Support.cs` | helper | `Support.NewFaker()` (Bogus) and `Support.AssertError(expected)` for asserting a LanguageExt `Error` by message |
| `Support.Infrastructure.cs` | helper | `Support.Infrastructure.UseContainer()` (Testcontainers SQL Server 2025 container) and `Support.Domain.NewIsinAsString()` (random ISIN-shaped string) |
| `Template.Core.TestCompanion.csproj` | project | Library project definition |

## How it works

`Support` is a `static partial class` in namespace `Template.Core.TestCompanion`, split across files by concern. `UseContainer()` only builds the container; unit tests do not start it. `test_integration/TestCompanion` has a same-named assembly used by the integration tests.

## Running

Not run directly; referenced by `test/BuildingBlocks/*` and `test/Modules/Ordering/TestCompanion`.

## Dependencies

- References: none (no project references)
- Packages: `Bogus`, `LanguageExt.Core`, `Microsoft.EntityFrameworkCore.InMemory`, `Microsoft.Extensions.Configuration(.Json)`, `Microsoft.Extensions.DependencyInjection(.Abstractions)`, `Microsoft.Extensions.Options`, `Testcontainers.MsSql`, `Selenium.WebDriver`, `coverlet.collector`, `xunit.v3`

## Extending

Only put helpers here that are independent of any module; module-specific helpers belong in `test/Modules/<Module>/TestCompanion`. Add new concerns as another `Support.<Concern>.cs` partial.
