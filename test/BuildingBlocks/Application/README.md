# BuildingBlocks Application Tests

Unit tests for the in-house mediator in `src/BuildingBlocks/Application/Communication`: request dispatch, pipeline behaviours, validation, domain event dispatch and DI registration. Assembly: `Acme.Core.BuildingBlocks.Application.Test`.

## Contents

| Item | Kind | Description |
|---|---|---|
| `Communication/MediatorTests.cs` | test class | Behaviour order (generic, then command, then handler), queries skip command behaviours, `ICommand` returning `Unit`, exception propagation, pipeline caching, `IRequest<T>` covariance |
| `Communication/DomainEventDispatcherTests.cs` | test class | All handlers per event run in order, events without handlers succeed, first failure stops dispatch, scoped dispatcher registered once, handlers discovered once per assembly |
| `Communication/FailureLoggingBehaviorTests.cs` | test class | Success logs nothing, expected codes log one Warning 1001, internal failures one Error 1002 with the exception, result unchanged, validation failure through the real mediator is logged (recording `ILogger<T>`) |
| `Communication/ServiceCollectionExtensionsTests.cs` | test class | `AddMediator` idempotency and default behaviour order, handler and FluentValidation validator discovery via `AddRequestHandlersFromAssembly`, duplicate handler for the same request throws |
| `Acme.Core.BuildingBlocks.Application.Test.csproj` | project | Test project definition |

## How it works

Each test builds a real `ServiceCollection` (`AddMediator()`, `AddRequestHandlersFromAssembly(...)`, `AddCommandPipelineBehavior<T>()`), creates a scope and resolves `IMediator` or `IDomainEventDispatcher`. Requests, handlers, validators and behaviours are nested test types; call order is captured in small `List<string>` log types (`CallLog`, `EventLog`) registered in the container. No infrastructure is required.

## Running

```powershell
dotnet test .\test\BuildingBlocks\Application\Acme.Core.BuildingBlocks.Application.Test.csproj
```

## Dependencies

- References: `src/BuildingBlocks/Application/Acme.Core.BuildingBlocks.Application.csproj`, `test/TestCompanion/Acme.Core.TestCompanion.csproj`
- Packages: `xunit.v3`, `xunit.runner.visualstudio`, `Microsoft.NET.Test.Sdk`, `Microsoft.Extensions.DependencyInjection`

## Extending

Mirror the `src/BuildingBlocks/Application` folder (`Communication`, ...). Nested handler types are picked up by assembly scanning, so a second handler for an existing request type in this assembly makes `AddRequestHandlersFromAssembly` throw; use `abstract` types (as `OtherPingHandler` does) when a type must be skipped by scanning.
