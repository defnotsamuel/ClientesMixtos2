using ClientesMixtos.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClientesMixtos.Repos
{
    public interface IClienteRepo
    {
        Task<List<Cliente>> GetAll();
        Task InsertCliente(Cliente cliente);
        Task UpdateCliente(Cliente cliente);
        Task DeleteCliente(Cliente cliente);
        Task UpdateFechaPago(Cliente cliente);
        Task UpdateFechaMarcada(Cliente cliente);
    }
}
