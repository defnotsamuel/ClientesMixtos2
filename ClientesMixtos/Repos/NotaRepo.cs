using ClientesMixtos.DB;
using ClientesMixtos.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClientesMixtos.Repos
{
    public class NotaRepo : INotaRepo
    {
        private readonly IMongoCollection<Nota> _collection;

        public NotaRepo(IMongoContext context)
        {
            _collection = context.GetCollection<Nota>("notas");
        }

        public Task<List<Nota>> GetByClienteId(ObjectId clienteId)
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

        public Task DeleteByClienteId(ObjectId clienteId)
        {
            var filtro = Builders<Nota>.Filter.Eq(n => n.ClienteId, clienteId);
            return _collection.DeleteManyAsync(filtro);
        }
    }
}
