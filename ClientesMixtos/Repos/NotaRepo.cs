using ClientesMixtos.DB;
using ClientesMixtos.Models;
using MongoDB.Driver;


namespace ClientesMixtos.Repos
{
    public class NotaRepo(MongoContext context)
    {
        private readonly IMongoCollection<Nota> _collection = context.GetCollection<Nota>("notas");

        public Task<List<Nota>> GetAll()
        {
            return _collection.Find(_ => true).ToListAsync();
        }

        public Task<List<Nota>> GetByClienteId(string clienteId)
        {
            var filtro = Builders<Nota>.Filter.Eq(n => n.ClienteId, clienteId);
            return _collection.Find(filtro).ToListAsync();
        }

        public Task InsertNota(Nota nota)
        {
            return _collection.InsertOneAsync(nota);
        }

        public Task UpdateNota(Nota nota)
        {
            var filtro = Builders<Nota>.Filter.Eq(n => n.Id, nota.Id);
            return _collection.ReplaceOneAsync(filtro, nota);
        }

        public Task DeleteNota(Nota nota)
        {
            var filtro = Builders<Nota>.Filter.Eq(n => n.Id, nota.Id);
            return _collection.DeleteOneAsync(filtro);
        }

        public Task DeleteByClienteId(string clienteId)
        {
            var filtro = Builders<Nota>.Filter.Eq(n => n.ClienteId, clienteId);
            return _collection.DeleteManyAsync(filtro);
        }
    }
}
