# Communication

In-process CQRS messaging (namespace `Acme.Core.BuildingBlocks.Application.Communication`, project `Acme.Core.BuildingBlocks.Application`): a custom mediator that returns LanguageExt `Fin<T>`, its pipeline behaviours, the domain event dispatcher, and DI registration.

## Contents

| Item | Kind | Description |
|---|---|---|
| `IRequest.cs` | interface `IRequest<out TResponse>` | Covariant marker for anything dispatched by the mediator |
| `ICommand.cs` | interfaces `ICommand<TResponse>`, `ICommand` | State-changing request; `ICommand` returns `Unit` |
| `IQuery.cs` | interface `IQuery<TResponse>` | Read-only request |
| `IRequestHandler.cs` | interface | `HandleAsync(request, ct) -> Task<Fin<TResponse>>` |
| `ICommandHandler.cs` | interfaces | `ICommandHandler<TCommand, TResponse>` and `ICommandHandler<TCommand>` (returns `Unit`) |
| `IQueryHandler.cs` | interface | `IQueryHandler<TQuery, TResponse>` |
| `IMediator.cs` | interface | `SendAsync<TResponse>(IRequest<TResponse>, ct)` |
| `Mediator.cs` | class | Default `IMediator`; delegates to the cached `RequestHandlerWrapper` |
| `RequestHandlerWrapper.cs` | internal classes | Per-(request, response) typed bridge, cached; builds and runs the pipeline |
| `RequestHandlerDelegate.cs` | delegate | `next()` step of the pipeline |
| `IPipelineBehavior.cs` | interface | Behaviour around every request (commands and queries) |
| `ICommandPipelineBehavior.cs` | interface | Behaviour around commands only (e.g. transactions, auditing, outbox) |
| `ValidationBehavior.cs` | sealed class | `IPipelineBehavior` running all FluentValidation validators of the request |
| `IDomainEventHandler.cs` | interface `IDomainEventHandler<in TEvent>` | Reacts in-process to a domain event after a successful save |
| `DomainEventDispatcher.cs` | sealed class (+ internal cached wrapper) | Implements `IDomainEventDispatcher` from BuildingBlocks.Domain |
| `ServiceCollectionExtensions.cs` | static class | `AddMediator`, `AddCommandPipelineBehavior<T>`, `AddRequestHandlersFromAssembly` |

## How it works

Dispatch (`Mediator.SendAsync`):
1. `RequestHandlerWrapper<TResponse>.For(request.GetType())` returns a `RequestHandlerWrapperImpl<TRequest, TResponse>` created once via reflection and cached in a static `ConcurrentDictionary`; later dispatches are plain virtual calls, so handler exceptions propagate unwrapped.
2. The wrapper resolves the single `IRequestHandler<TRequest, TResponse>` from the current scope (`GetRequiredService`; missing handler throws).
3. The pipeline is built inside-out, each group reversed so the first registered behaviour is outermost in its group. Resulting order:
   - all `IPipelineBehavior`s (outermost, e.g. `ValidationBehavior`)
   - all `ICommandPipelineBehavior`s (only if `TRequest` is an `ICommand<TResponse>`)
   - the handler

`ValidationBehavior`: resolves every `IValidator<TRequest>`; if none, calls `next()`. Otherwise runs all validators, collects all error messages and, if any, returns `ValidationError` (code `validation`) with messages joined by `"; "` without calling the handler.

Domain events: `DomainEventDispatcher.DispatchAsync` processes events in order; per event, a cached `DomainEventHandlerWrapperImpl<TEvent>` resolves all `IDomainEventHandler<TEvent>` (runtime type) from the current scope and runs them in registration order. The first failed `Fin` stops dispatch and is returned. An event without handlers succeeds.

Registration:
- `AddMediator()` (host, once; idempotent): `TryAddScoped<IMediator, Mediator>`, `TryAddScoped<IDomainEventDispatcher, DomainEventDispatcher>`, `ValidationBehavior` as `IPipelineBehavior` via `TryAddEnumerable`.
- `AddCommandPipelineBehavior<T>()`: scoped, idempotent per type.
- `AddRequestHandlersFromAssembly(assembly)` scans concrete classes and registers (scoped):
  - every closed `IRequestHandler<,>`: exactly one handler per request type across all modules; a different implementation for an already-registered request throws `InvalidOperationException`; re-scanning the same assembly is a no-op;
  - every `IDomainEventHandler<>` (many per event allowed);
  - every FluentValidation `IValidator<>`.

```csharp
public sealed record PingQuery : IQuery<string>;
internal sealed class PingHandler : IQueryHandler<PingQuery, string>
{
    public Task<Fin<string>> HandleAsync(PingQuery q, CancellationToken ct = default) =>
        Task.FromResult(Fin<string>.Succ("pong"));
}
// host: services.AddMediator().AddRequestHandlersFromAssembly(typeof(PingHandler).Assembly);
var result = await mediator.SendAsync(new PingQuery(), ct); // Fin<string>
```

## Dependencies
- References: FluentValidation, Microsoft.Extensions.DependencyInjection.Abstractions, BuildingBlocks.Domain (`DomainEvent`, `IDomainEventDispatcher`, `Errors`).
- Used by: `Acme.Core.Api` (`AddMediator`, gRPC services use `IMediator`), Ordering Application (handlers, `AddRequestHandlersFromAssembly`), Ordering Contracts (`ICommand`/`IQuery`), `UnitOfWork<TContext>` via `IDomainEventDispatcher`, `test/BuildingBlocks/Application/Communication`.

## Extending
- New use case: define an `ICommand<T>`/`IQuery<T>` in the module's Contracts and an `ICommandHandler`/`IQueryHandler` in its Application project; it is picked up by `AddRequestHandlersFromAssembly`. Add an `AbstractValidator<TRequest>` to get validation automatically.
- New cross-cutting step: implement `IPipelineBehavior` (all requests) and register it with `TryAddEnumerable`, or implement `ICommandPipelineBehavior` and register with `AddCommandPipelineBehavior<T>()`. Registration order defines nesting.
- New event reaction: implement `IDomainEventHandler<TEvent>` in a module's Application project.
