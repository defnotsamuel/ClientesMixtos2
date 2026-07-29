using ClientesMixtos.Configuration;
using MongoDB.Driver;

namespace ClientesMixtos.DB
{
    public class MongoContext : IMongoContext
    {
        public IMongoDatabase Database { get; private set; }

        public MongoContext(GlobalConfig config)
        {
            var client = new MongoClient(config.ConnectionString);
            Database = client.GetDatabase(config.DatabaseName);
        }

        public IMongoCollection<T> GetCollection<T>(string collectionName)
        {
            return Database.GetCollection<T>(collectionName);
        }
    }
}
