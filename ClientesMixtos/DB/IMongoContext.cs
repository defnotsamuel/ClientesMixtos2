using MongoDB.Driver;

namespace ClientesMixtos.DB
{
    public interface IMongoContext
    {
        IMongoDatabase Database { get; }
        IMongoCollection<T> GetCollection<T>(string collectionName);
    }
}
