# Validation

Options validation with FluentValidation (namespace `Acme.Core.BuildingBlocks.Application.Validation`, project `Acme.Core.BuildingBlocks.Application`). Lets configuration classes bound via `IOptions<T>` be validated by an `IValidator<T>` instead of data annotations.

## Contents

| Item | Kind | Description |
|---|---|---|
| `FluentValidateOptions.cs` | class `FluentValidateOptions<TOptions>` | `IValidateOptions<TOptions>` that runs the registered `IValidator<TOptions>` |
| `OptionsBuilderExtensions.cs` | static class | `ValidateFluentValidation()` on `OptionsBuilder<TOptions>` |

## How it works

1. `ValidateFluentValidation()` registers a singleton `IValidateOptions<TOptions>` = `FluentValidateOptions<TOptions>(serviceProvider, builder.Name)`.
2. On validation, if the builder was for a named option and the name does not match, it returns `Skip`.
3. It creates a DI scope and resolves `IValidator<TOptions>` (required; missing validator throws).
4. Valid: `Success`. Invalid: `Fail` with one message per failure: `Validation failed for {Type}.{Property} with the error: {Message}`.
5. Combine with `ValidateOnStart()` to fail application startup on invalid configuration.

```csharp
services.AddSingleton<IValidator<PersistenceOptions>, PersistenceOptionsValidator>();
services.AddOptions<PersistenceOptions>()
    .Bind(configuration.GetSection("Persistence"))
    .ValidateFluentValidation()
    .ValidateOnStart();
```

## Dependencies
- References: FluentValidation, Microsoft.Extensions.Options, Microsoft.Extensions.DependencyInjection.Abstractions.
- Used by: `Acme.Core.Ordering.Persistence` (`ServiceCollectionExtensions.AddOrderingPersistence` for `PersistenceOptions`).

## Extending
- For a new options class: write an `AbstractValidator<TOptions>`, register it as `IValidator<TOptions>`, and call `.ValidateFluentValidation()` on its options builder.
