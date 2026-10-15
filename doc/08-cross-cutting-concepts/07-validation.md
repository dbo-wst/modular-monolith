# 8.7 Validation

Input validation with FluentValidation: validators live next to their handler (e.g. `Orders/CreateOrder/CreateOrderValidator`) and are executed by `ValidationBehavior` in the mediator pipeline. Business invariants are enforced by the aggregates.

## Mediator pipeline

`IMediator.SendAsync` runs, from outermost to innermost:

1. every `IPipelineBehavior` (all requests; first registered runs first) — `ValidationBehavior`, registered by `AddMediator`, so invalid input never reaches later steps;
2. every `ICommandPipelineBehavior` (commands only, i.e. requests implementing `ICommand<T>`; registered via `AddCommandPipelineBehavior<T>()`) — the place for transactions, auditing or outbox flushing;
3. the request's single handler.

Behaviours call `next` (`RequestHandlerDelegate<T>`) to continue. Dispatch uses a strongly-typed handler wrapper cached per request type, so there is no per-call reflection and handler exceptions propagate unwrapped. Commands without a result implement `ICommand` / `ICommandHandler<TCommand>` and return `Unit`.
