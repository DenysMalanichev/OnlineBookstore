namespace Recommendations.Tests.IntegrationTests;

[CollectionDefinition("MongoDB Collection")]
public class MongoDbCollection : ICollectionFixture<MongoDbFixture>
{
    // This class doesn't need to do anything.
    // It's just the way xUnit works with collection fixtures
}
