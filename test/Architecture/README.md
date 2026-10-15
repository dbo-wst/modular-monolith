# Architecture Tests

Guard rails for the Clean Architecture dependency rule, written with NetArchTest. Assembly: `Template.Core.Architecture.Test`.

## Contents

| Item | Kind | Description |
|---|---|---|
| `ArchitectureTests.cs` | test class | Layer dependency rules for the Ordering module, BuildingBlocks and the gateway host |
| `Template.Core.Architecture.Test.csproj` | project | Test project definition |

## How it works

`ArchitectureTests` loads the real assemblies via marker types (`Order`, `IOrderingReadStore`, `Contracts.Dto.Order`, `IHasDomainEvents`, `IMediator`, `Persistence.UnitOfWork`, `Template.Api.Segments`) and asserts with `Types.InAssembly(...).ShouldNot().HaveDependencyOnAny(...)`:

- Ordering Domain depends neither on Application, Contracts nor adapter assemblies, and not on EF Core, `Microsoft.Extensions.DependencyInjection` or FluentValidation.
- Ordering Contracts depend on no other module layer.
- Ordering Application depends neither on adapters nor on EF Core.
- Adapter assemblies (the `Adapters` array, currently only Persistence) live in the `Template.Core.Ordering.` namespace and do not depend on each other.
- The gateway (`Template.Api`) only knows the module through Contracts/Protocol, not Domain, Application or adapters.
- BuildingBlocks Domain does not depend on infrastructure frameworks or on BuildingBlocks Application/Persistence; BuildingBlocks Application does not depend on EF Core or BuildingBlocks Persistence.
- Types in Contracts namespaces ending in `Commands` or `Queries` are sealed.

Failure messages list the offending type names. No infrastructure is required.

## Running

```powershell
dotnet test .\test\Architecture\Template.Core.Architecture.Test.csproj
```

## Dependencies

- References: `src/Modules/Ordering/Domain`, `src/Modules/Ordering/Application`, `src/Modules/Ordering/Contracts`, `src/Modules/Ordering/Persistence`, `src/Host/Template.Api`
- Packages: `NetArchTest.Rules`, `xunit.v3`, `xunit.runner.visualstudio`, `Microsoft.NET.Test.Sdk`

## Extending

- New adapter project in Ordering (Messaging, ExternalServices, ...): add a project reference and add its assembly to the `Adapters` array.
- New module: add project references and a new set of rules (or parameterise `Root` and the marker types) for that module.
