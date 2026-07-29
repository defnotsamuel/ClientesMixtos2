using ClientesMixtos.Models;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClientesMixtos.Repos
{
    public interface IPagoRepo
    {
        Task<List<Pago>> GetByClienteId(ObjectId clienteId);
        Task<Pago?> GetByFecha(ObjectId clienteId, DateTime fechaPagada);
        Task Insert(Pago pago);
        Task InsertMany(IEnumerable<Pago> pagos);
        Task Delete(ObjectId id);
    }
}
