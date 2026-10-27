# CLAUDE.md — Unit and architecture tests

Fast tests. They must not need Docker, SQL Server or the network. Stack: xUnit v3, FakeItEasy, and the EF Core
in-memory provider where a DbContext is unavoidable.

## Layout

The tree mirrors `src`:
- `test/BuildingBlocks/{Domain,Application,Observability}`
- `test/Host/Acme.Core.Api` (background jobs; internals via `InternalsVisibleTo`)
- `test/Modules/<Module>/{Domain,Application,TestCompanion}`
- `test/Architecture`
- `test/TestCompanion` for shared helpers

Each slice's tests live in a folder named after the slice (`Application/Orders/CreateOrder/CreateOrderHandlerTests.cs`).

## Rules

- **Domain tests:**
  - Cover every invariant of `Create`.
  - Cover every allowed and forbidden status transition.
  - Check that the expected events are raised, and that a no-op change raises nothing.
- **Handler tests:**
  - Fake the ports (`I<Aggregate>Repository`, `IUnitOfWork`, `I<Module>ReadStore`) with FakeItEasy.
  - Assert on the returned `Fin`, including the error type and `ErrorCode`, not on messages.
  - Cover the success path, validation failure, not-found and persistence failure.
- **Pipeline tests:** go through the real mediator (`AddMediator` + `Add<Module>Application`). This proves validators
  and handlers are registered. Use `RecordingDomainEventDispatcher` from the module TestCompanion for event assertions.
- **Architecture tests** (`test/Architecture`):
  - Add every new module and adapter to the assembly lists.
  - Rules must not pass vacuously.
  - Never relax a rule to make code pass.
- Naming: `Method_Scenario_Expectation`. Use one assert concept per test, and use `[Theory]` for value tables.
- Do not use `Thread.Sleep`, real clocks in assertions, or shared mutable static state.

## Run

```bash
dotnet test Acme.Core.slnx --filter "FullyQualifiedName!~Ordering.Application.IntegrationTest&FullyQualifiedName!~Persistence.IntegrationTest"
```
