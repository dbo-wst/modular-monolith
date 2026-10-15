# 4. Solution Strategy

| Goal | Approach |
|------|----------|
| Changeability and clear boundaries | Modular monolith, one module per bounded context ([ADR 0001](../09-architecture-decisions/0001-modular-monolith-clean-architecture-cqrs.md)) |
| Independence from frameworks | Clean Architecture, dependency rule enforced by `test/Architecture` |
| Fast, local changes | Vertical slices per use case |
| Scalable reads | CQRS with a separate read store |
| Explicit failures | `Fin<T>` results instead of exceptions for expected errors |
