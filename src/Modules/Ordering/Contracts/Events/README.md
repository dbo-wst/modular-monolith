# Contract Events

Placeholder for integration events of the Ordering module (Contracts layer). The folder currently contains only a `.gitkeep`; no event types exist yet.

## Contents

| Item | Kind | Description |
|---|---|---|
| `.gitkeep` | file | Keeps the empty folder in git. |

## How it works

- Intended content: public, primitive-only event records that other modules or hosts may subscribe to (e.g. a published counterpart of a domain event), independent of domain types.
- Not to be confused with the domain events in `Domain/Orders/Events` and `Domain/OrderItems/Events`, which carry domain types and are dispatched in-process by the unit of work. Nothing currently maps domain events to contract events.

## Dependencies

- References: none.
- Used by: nothing yet.

## Extending

Add `sealed record`s with primitive/DTO fields here; publish them from an `IDomainEventHandler<TDomainEvent>` in Application (or a messaging adapter) so that consumers depend only on Contracts.
