using Mongo2Go;
using MongoDB.Driver;

namespace Recommendations.Tests.IntegrationTests;
public class MongoDbFixture : IDisposable
{
    private readonly MongoDbRunner _runner;

    public IMongoDatabase Database { get; }
    public MongoClient Client { get; }
    public string ConnectionString { get; }

    public MongoDbFixture()
    {
        _runner = MongoDbRunner.Start();
        ConnectionString = _runner.ConnectionString;
        Client = new MongoClient(ConnectionString);
        Database = Client.GetDatabase("BookRecommendationTests");
    }

    public void Dispose()
    {
        _runner?.Dispose();
    }
}
