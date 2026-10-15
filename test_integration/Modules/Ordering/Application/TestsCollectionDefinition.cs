using Acme.Core.Ordering.TestCompanion;

namespace Acme.Core.Ordering.Application.IntegrationTest;

[CollectionDefinition(IntegrationTestsFixture.CollectionName)]
public sealed class TestsCollectionDefinition : ICollectionFixture<IntegrationTestsFixture>;


