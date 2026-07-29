using ClientesMixtos.DB;
using ClientesMixtos.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClientesMixtos.Repos
{
    public class PagoRepo : IPagoRepo
    {
        private readonly IMongoCollection<Pago> _collection;

        public PagoRepo(IMongoContext context)
        {
            _collection = context.Database.GetCollection<Pago>("Pagos");
        }

        public async Task<List<Pago>> GetByClienteId(ObjectId clienteId)
        {
            return await _collection
                .Find(x => x.ClienteId == clienteId)
                .SortBy(x => x.FechaPagada)
                .ToListAsync();
        }

        public async Task<Pago?> GetByFecha(ObjectId clienteId, DateTime fechaPagada)
        {
            var inicio = new DateTime(fechaPagada.Year, fechaPagada.Month, 1);
            var fin = inicio.AddMonths(1);

            return await _collection.Find(x =>
                (x.ClienteId) == clienteId &&
                x.FechaPagada >= inicio &&
                x.FechaPagada < fin)
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
