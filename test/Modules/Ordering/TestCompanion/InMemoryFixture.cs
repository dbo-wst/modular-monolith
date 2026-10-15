using Acme.Core.Ordering.Persistence;

namespace Acme.Core.Ordering.TestCompanion;

public sealed class InMemoryFixture
{
    public Bogus.Faker Faker = new();
    public required IOrderingDbContextFactory ContextFactory { get; init; } =
        Support.Infrastructure.DbContextFactory();
}


