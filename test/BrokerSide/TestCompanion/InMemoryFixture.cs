using Wst.Trios.Core.BrokerSide.Persistence;

namespace Wst.Trios.Core.BrokerSide.TestCompanion;

public sealed class InMemoryFixture
{
    public Bogus.Faker Faker = new();
    public required IBrokerSideDbContextFactory ContextFactory { get; init; } =
        Support.Infrastructure.DbContextFactory();
}


