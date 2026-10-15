using FakeItEasy;
using Microsoft.EntityFrameworkCore;
using Acme.Core.Ordering.Persistence;
using Acme.Core.Ordering.Persistence.DbContext;
using Acme.Core.BuildingBlocks.Domain.Functional;

namespace Acme.Core.Ordering.TestCompanion;

public static class Support
{
    public static class Infrastructure
    {
        public static IOrderingDbContextFactory DbContextFactory() => InMemoryOptions(Guid.NewGuid())
            .Then(o => ContextFactory(() => new OrderingDbContext(o)));
        
        private static DbContextOptions<OrderingDbContext> InMemoryOptions(Guid contextId) =>
            new DbContextOptionsBuilder<OrderingDbContext>()
                .UseInMemoryDatabase(databaseName: contextId.ToString()).Options;

        private static IOrderingDbContextFactory ContextFactory(Func<OrderingDbContext> contextFunc)
        {
            var contextFactory = A.Fake<IOrderingDbContextFactory>();
            A.CallTo(() => contextFactory.CreateDbContext())
                .ReturnsLazily(contextFunc);
            A.CallTo(() => contextFactory.CreateDbContextAsync(A<CancellationToken>._))
                .ReturnsLazily(contextFunc);
            return contextFactory;
        }
    }
}
