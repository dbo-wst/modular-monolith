namespace Acme.Api.IntegrationTest;

[CollectionDefinition(CollectionName, DisableParallelization = true)]
public sealed class TestsCollectionDefinition
{
    public const string CollectionName = "Acme.Api.IntegrationTest";
}
