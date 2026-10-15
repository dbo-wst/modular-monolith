# Contract Responses

Placeholder folder in the Ordering Contracts layer. It currently contains only a `.gitkeep`; no response types exist yet.

## Contents

| Item | Kind | Description |
|---|---|---|
| `.gitkeep` | file | Keeps the empty folder in git. |

## How it works

- Command and query outputs currently live in [`../Results`](../Results/README.md); transport-specific request/response types live in the hosts (`src/Host/*/Prelaunch/Ordering`) and in `src/Protocol`.
- Intended content: public response shapes of the module that are not tied to a single command/query result (for example shared paged or composite responses), if such a need arises.

## Dependencies

- References: none.
- Used by: nothing yet.

## Extending

Add `sealed record`s built from `../Dto` types only; keep them free of domain and transport types.
