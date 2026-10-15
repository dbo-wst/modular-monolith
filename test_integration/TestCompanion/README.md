# Test Companion (integration, shared)

Module-agnostic helpers shared by the integration test projects. Assembly: `Template.Core.TestCompanion` (library, contains no tests).

## Contents

| Item | Kind | Description |
|---|---|---|
| `Support.Infrastructure.cs` | helper | `Support.Infrastructure.UseContainer()`: builds a Testcontainers `MsSqlContainer` (`mcr.microsoft.com/mssql/server:2025-latest`, password `MyP@ssword`, cleanup enabled) |
| `Template.Core.TestCompanion.csproj` | project | Library project definition |

## How it works

`Support` and `Support.Infrastructure` are `static partial` classes in namespace `Template.Core.TestCompanion`. `UseContainer()` only builds the container; the caller (e.g. `IntegrationTestsFixture`) starts and disposes it, which requires Docker. This assembly has the same name as `test/TestCompanion` but is a separate project.

## Running

Not run directly; referenced by `test_integration/Modules/Ordering/TestCompanion`.

## Dependencies

- References: none (no project references)
- Packages: `Bogus`, `LanguageExt.Core`, `Microsoft.EntityFrameworkCore.InMemory`, `Microsoft.Extensions.Configuration(.Json)`, `Microsoft.Extensions.DependencyInjection(.Abstractions)`, `Microsoft.Extensions.Options`, `Testcontainers.MsSql`, `coverlet.collector`, `xunit.v3`, `xunit.v3.assert`

## Extending

Keep this module-agnostic; put module-specific fixtures in `test_integration/Modules/<Module>/TestCompanion`. Add new concerns as further `Support` partials.
