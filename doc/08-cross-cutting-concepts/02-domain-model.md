# 8.2 Domain Model

Aggregates (`Order`, `OrderItem`) are the write model: they enforce invariants, raise domain events (`AggregateRoot`) and are created with `Create` or loaded with `Rehydrate`. Value objects live in `Domain/ValueObjects`. The sample module `Ordering` demonstrates the pattern; replace it with your own domain.
