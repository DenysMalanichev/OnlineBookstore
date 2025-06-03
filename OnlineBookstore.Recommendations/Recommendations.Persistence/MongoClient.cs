using MongoDB.Driver;
using Recommendations.Abstractions.Entities;

namespace Recommendations.Persistence
{
    public class MongoDbContext : IDisposable
    {
        private readonly IMongoClient _client;
        private readonly IMongoDatabase _database;

        public MongoDbContext(string connectionString, string databaseName)
        {
            _client = new MongoClient(connectionString);
            _database = _client.GetDatabase(databaseName);
        }

        public IMongoCollection<BookPortrait> BookPortraits => _database.GetCollection<BookPortrait>("BookPortraits");
        public IMongoCollection<UserPortrait> UserPortraits => _database.GetCollection<UserPortrait>("UserPortraits");
        
        public void Dispose()
        {
            _client.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
