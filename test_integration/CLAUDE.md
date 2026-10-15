# CLAUDE.md — Integration tests

Tests that go through real infrastructure or hosts.

- `Host/Acme.Core.Api.IntegrationTest`:
  - Uses `WebApplicationFactory` (`CoreApiFactory`) with fakes for the repositories, unit of work and read store.
  - Loads the test project's `appsettings.json` so startup options validation passes.
  - Needs no database.
- `Host/Acme.Api.IntegrationTest`: the gateway via `GatewayApiFactory` with faked gRPC clients. Asserts routes,
  status codes, the `Location` header and the error-to-HTTP mapping.
- `Modules/<Module>/{Application,Persistence}`:
  - Run against real SQL Server through `IntegrationTestsFixture`.
  - Use Testcontainers by default, or `Persistence:ConnectionString` from a local, uncommitted `appsettings.test.json`.
  - Migrations are applied automatically.

## Rules

- Persistence tests cover:
  - Dbo/aggregate mapping round-trips.
  - Repository CRUD, including conflict and not-found.
  - Read-store paging limits.
  - Events dispatched only after a successful save, and not when the save fails.
- Each test creates its own data with unique ids and must not depend on execution order or on other tests' rows.
- Assert on error types and codes, never on message text.
- Never commit connection strings or secrets. `appsettings.test.json` stays local.
- If Docker or SQL Server is not available, say that these tests were not run; do not mark them skipped in code.

## Run

```bash
dotnet test test_integration/Host/Acme.Core.Api.IntegrationTest
dotnet test test_integration/Host/Acme.Api.IntegrationTest
dotnet test test_integration/Modules/Ordering/Persistence     # needs Docker or SQL Server
dotnet test test_integration/Modules/Ordering/Application     # needs Docker or SQL Server
```
