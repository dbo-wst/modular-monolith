# CLAUDE.md — Modules

Each folder here is one bounded context. `Ordering` is the reference module: copy its patterns exactly.

## Module shape

```
<Module>/
  Domain/        Acme.Core.<Module>.Domain        <Aggregates>/{<Aggregate>.cs, <Aggregate>Id.cs, <Status>.cs, I<Aggregate>Repository.cs, Events/}, ValueObjects/
  Application/   Acme.Core.<Module>.Application   <Aggregates>/<Slice>/{<Slice>Handler.cs, <Slice>Validator.cs}, <Aggregates>/Mappers/, Abstractions/I<Module>ReadStore.cs, ServiceCollectionExtensions.cs
  Contracts/     Acme.Core.<Module>.Contracts     Commands/, Queries/, Results/, Dto/ (public API of the module)
  Persistence/   Acme.Core.<Module>.Persistence   DbContext/, <Aggregates>/{Dbo, Configuration, Mappers/, Repository}, ReadStore, UnitOfWork, PersistenceOptions(+Validator), Migrations/
  <Adapter>/     one project per further adapter
```

## Domain rules

- Aggregates:
  - `sealed class <Aggregate> : AggregateRoot<<Aggregate>Id>` with a private constructor and `private set` properties.
  - `static Fin<<Aggregate>> Create(...)` enforces every invariant and raises `<Aggregate>Created`.
  - `static <Aggregate> Rehydrate(...)` is used only by persistence mappers. It never validates and never raises events.
- State changes are behaviour methods (`ChangeDetails`, `Start`, `Complete`, `Cancel`, ...):
  - Return `Fin<T>` when they can fail; a forbidden status transition is a `ValidationError`.
  - Return plain values when nothing can fail.
  - Raise events only when something actually changed; `<Aggregate>Updated` carries the current state.
  - `Delete()` raises `<Aggregate>Deleted`.
- Status lifecycles use the **State pattern** (see `Ordering/Domain/Orders`):
  - `I<Aggregate>State.cs` next to the aggregate: one method per transition taking the aggregate and returning
    `Fin<Unit>`, plus `static From(<Status>)` for rehydration.
  - Concrete states in `<Aggregates>/States/<Status><Aggregate>State.cs`: `sealed`, stateless singletons (`Instance`);
    a forbidden transition returns a `ValidationError`.
  - The aggregate is the Context: it holds the current state, delegates its behaviour methods to it, and exposes
    `internal TransitionTo(next)`, which swaps the state and raises `<Aggregate>Updated`.
  - The status enum stays the persisted and wire value; persistence never sees state objects.
- Strongly typed ids (`<Aggregate>Id.From(Guid)`). Value objects are records in `ValueObjects/`:
  - `Create(...)` returns `Fin<T>` and validates.
  - `From(...)` wraps without validation; the nullable `From` overload returns `null` for `null` input.
- Other aggregates are referenced by id only, never by object reference.
- Repository ports live next to their aggregate:
  - `AddAsync`, `UpdateAsync`, `DeleteAsync(aggregate)`, `GetByIdAsync(id)`, all returning `Fin<T>`.
  - No list or search methods; those belong to the read store.
- No EF Core, DI, FluentValidation, DTOs or Contracts types in Domain.

## Application rules (vertical slices + CQRS)

- One folder per use case: `<Aggregates>/<Slice>/`. A slice contains its `internal sealed` handler and an optional
  `internal sealed` FluentValidation validator. Nothing slice-specific goes elsewhere.
- **Commands** (`ICommandHandler<TCommand, TResult>`):
  1. Build value objects and the aggregate, or load it via `GetByIdAsync`.
  2. Call the aggregate behaviour.
  3. Persist through the repository.
  4. Call `IUnitOfWork.SaveChangesAsync`.
  5. Map to the `<Slice>Result`.
  Compose this as one `Fin` pipeline.
- **Queries** (`IQueryHandler<TQuery, TResult>`) use only `I<Module>ReadStore`, which returns DTOs directly. Queries
  never load aggregates, never call repositories, and never change state.
- Validators check input shape (required, length, range). Business invariants stay in the domain.
- DTO mapping lives in `<Aggregates>/Mappers` (`IOnewayMap`); no AutoMapper.
- Event reactions implement `IDomainEventHandler<TEvent>` in Application. They are registered automatically.
- `ServiceCollectionExtensions.Add<Module>Application()` only calls `AddRequestHandlersFromAssembly`. Never register
  `IMediator` in a module.

## Contracts rules

- `sealed record <Slice>Command : ICommand<<Slice>Result>` / `<Slice>Query : IQuery<<Slice>Result>`. Wire-facing
  inputs may be nullable; validation decides.
- Results and DTOs are sealed records holding primitives, `Guid`, `DateOnly` and enums only. Never expose Domain types.
- Contracts is the only project other modules and hosts may reference.

## Persistence rules

- EF Core lives only here:
  - Dbo classes plus `IEntityTypeConfiguration`.
  - The module's own schema and its own migrations history table (`Constants.Schema`, `Constants.MigrationTableName`).
  - No navigation into other modules' tables.
- Mappers convert Dbo to aggregate with `Rehydrate`, and aggregate to Dbo.
- Repositories:
  - Call `events.Track(aggregate)` on add, update and delete.
  - Return `ConflictError.AlreadyExists` or `NotFoundError.ForEntity` instead of throwing.
  - Use `AsNoTracking` for reads.
- `ReadStore` projects straight to Contracts DTOs and clamps paging: page 1–1000, size 10 by default, 100 at most.
- `UnitOfWork : UnitOfWork<<Module>DbContext>`. Register one DbContext per scope, shared by the repositories and the unit of work.
- `Add<Module>Persistence(IConfiguration)` binds `Persistence` options with `ValidateFluentValidation().ValidateOnStart()`.
- After any model change:
  1. Add a migration (`add-migrations.sh` or the command in `Persistence/readme.md`).
  2. Make sure `has-pending-model-changes` reports no changes.
  3. Never edit an applied migration.

## Adding a module

1. Create the four projects (plus adapters) and the `test`/`test_integration` projects. Add them all to `Acme.Core.slnx`.
2. Implement `Add<Module>Application()` and `Add<Module>Persistence()`.
3. Add a host extension `Add<Module>Module()` in `src/Host/Acme.Core.Api/Prelaunch/<Module>`, called after `AddMediator()`.
4. Add the module to the assembly lists in `test/Architecture/ArchitectureTests.cs`.
5. Add its migration to `add-migrations.sh`.
6. Add READMEs and update `doc/05-building-block-view`.
