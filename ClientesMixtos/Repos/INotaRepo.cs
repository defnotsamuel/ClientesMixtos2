using ClientesMixtos.Models;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClientesMixtos.Repos
{
    public interface INotaRepo
    {
        Task<List<Nota>> GetByClienteId(ObjectId clienteId);
        Task InsertNota(Nota nota);
        Task UpdateNota(Nota nota);
        Task DeleteNota(Nota nota);
        Task DeleteByClienteId(ObjectId clienteId);
    }
}
