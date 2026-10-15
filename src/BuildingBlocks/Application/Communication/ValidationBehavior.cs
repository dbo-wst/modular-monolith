using FluentValidation;
using LanguageExt;
using Microsoft.Extensions.DependencyInjection;
using Template.Core.BuildingBlocks.Domain.Errors;

namespace Template.Core.BuildingBlocks.Application.Communication;

/// <summary>Runs every registered FluentValidation validator for the request before its handler executes.</summary>
public sealed class ValidationBehavior(IServiceProvider provider) : IPipelineBehavior
{
    public async Task<Fin<TResponse>> HandleAsync<TResponse>(
        IRequest<TResponse> request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct = default)
    {
        var validatorType = typeof(IValidator<>).MakeGenericType(request.GetType());
        var validators = provider.GetServices(validatorType).OfType<IValidator>().ToList();
        if (validators.Count == 0)
        {
            return await next();
        }

        var context = new ValidationContext<object>(request);
        var failures = new List<string>();
        foreach (var validator in validators)
        {
            var result = await validator.ValidateAsync(context, ct);
            failures.AddRange(result.Errors.Select(e => e.ErrorMessage));
        }

        return failures.Count == 0
            ? await next()
            : Fin<TResponse>.Fail(ValidationError.New(string.Join("; ", failures)));
    }
}
