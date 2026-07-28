using ClientesMixtos.DB;
using ClientesMixtos.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClientesMixtos.Repos
{
    public class PagoRepo(MongoContext context)
    {
        private readonly IMongoCollection<Pago> _collection = context.Database.GetCollection<Pago>("Pagos");

        public async Task<List<Pago>> GetByClienteId(string clienteId)
        {
            return await _collection
                .Find(x => x.ClienteId == clienteId)
                .SortBy(x => x.FechaPagada)
                .ToListAsync();
        }

        public async Task<Pago?> GetByFecha(string clienteId, DateTime fechaPagada)
        {
            return await _collection
                .Find(x =>
                    x.ClienteId == clienteId &&
                    x.FechaPagada.Year == fechaPagada.Year &&
                    x.FechaPagada.Month == fechaPagada.Month)
                .FirstOrDefaultAsync();
        }

        public async Task Insert(Pago pago)
        {
            await _collection.InsertOneAsync(pago);
        }

        public async Task InsertMany(IEnumerable<Pago> pagos)
        {
            await _collection.InsertManyAsync(pagos);
        }

        public async Task Delete(ObjectId id)
        {
            await _collection.DeleteOneAsync(x => x.Id == id);
        }
    }
}
