using ClientesMixtos.Models;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClientesMixtos.Services
{
    public interface INotaService
    {
        Task<List<Nota>> FromClient(Cliente cliente);
        Task Insert(Nota nota, Cliente cliente);
        Task Delete(Nota nota);
        Task DeleteByClienteId(ObjectId clienteId);
        Task Update(Nota nota);
    }
}
