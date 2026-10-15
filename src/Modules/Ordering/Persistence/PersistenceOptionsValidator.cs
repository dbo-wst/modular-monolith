using FluentValidation;

namespace Acme.Core.Ordering.Persistence;

public sealed class PersistenceOptionsValidator : AbstractValidator<PersistenceOptions>
{
    public PersistenceOptionsValidator()
    {
        RuleFor(x => x.ConnectionString).NotEmpty();
    }
}
