using ClientesMixtos.DB;
using ClientesMixtos.Models;
using MongoDB.Driver;

namespace ClientesMixtos.Repositories
{
    public class PagosClienteRepository(MongoContext context)
    {
        private readonly IMongoCollection<PagosCliente> _collection = context.GetCollection<PagosCliente>("pagos");

        public async Task<PagosCliente?> GetByClienteId(string clienteId)
        {
            var filter = Builders<PagosCliente>.Filter.Eq(p => p.ClienteId, clienteId);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public Task Insert(PagosCliente pagosCliente)
        {
            return _collection.InsertOneAsync(pagosCliente);
        }

        public Task Update(PagosCliente pagosCliente)
        {
            return _collection.ReplaceOneAsync(x => x.Id == pagosCliente.Id, pagosCliente);
        }

        public Task DeleteByClienteId(string clienteId)
        {
            var filter = Builders<PagosCliente>.Filter.Eq(p => p.ClienteId, clienteId);
            return _collection.DeleteOneAsync(filter);
        }
    }
}
