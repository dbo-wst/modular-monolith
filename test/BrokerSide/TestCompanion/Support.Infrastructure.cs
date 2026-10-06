using FakeItEasy;
using Microsoft.EntityFrameworkCore;
using Wst.Trios.Core.BrokerSide.Persistence;
using Wst.Trios.Core.BrokerSide.Persistence.DbContext;
using Wst.Trios.Core.Shared.Kernel.Functional;

namespace Wst.Trios.Core.BrokerSide.TestCompanion;

public static class Support
{
    public static class Infrastructure
    {
        public static IBrokerSideDbContextFactory DbContextFactory() => InMemoryOptions(Guid.NewGuid())
            .Then(o => ContextFactory(() => new BrokerSideDbContext(o)));
        
        private static DbContextOptions<BrokerSideDbContext> InMemoryOptions(Guid contextId) =>
            new DbContextOptionsBuilder<BrokerSideDbContext>()
                .UseInMemoryDatabase(databaseName: contextId.ToString()).Options;

        private static IBrokerSideDbContextFactory ContextFactory(Func<BrokerSideDbContext> contextFunc)
        {
            var contextFactory = A.Fake<IBrokerSideDbContextFactory>();
            A.CallTo(() => contextFactory.CreateDbContext())
                .ReturnsLazily(contextFunc);
            A.CallTo(() => contextFactory.CreateDbContextAsync(A<CancellationToken>._))
                .ReturnsLazily(contextFunc);
            return contextFactory;
        }
    }
}
