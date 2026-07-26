using MongoDB.Driver;

namespace ClientesMixtos.DB
{
    public class MongoContext
    {
        public IMongoDatabase Database { get; private set; }

        public MongoContext()
        {
            var connectionString = Configuration.GlobalConfig.ConnectionString();
            var databaseName = Configuration.GlobalConfig.DatabaseName();

            var client = new MongoClient(connectionString);

            Database = client.GetDatabase(databaseName);
        }

        public IMongoCollection<T> GetCollection<T>(string collectionName)
        {
            return Database.GetCollection<T>(collectionName);
        }

    }
}
