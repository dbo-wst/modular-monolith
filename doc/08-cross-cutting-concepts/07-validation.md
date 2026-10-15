# 8.7 Validation

Input validation with FluentValidation: validators live next to their handler (e.g. `Orders/CreateOrder/CreateOrderValidator`) and are executed by `ValidationBehavior` in the mediator pipeline. Business invariants are enforced by the aggregates.
