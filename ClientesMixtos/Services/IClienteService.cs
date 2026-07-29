using ClientesMixtos.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClientesMixtos.Services
{
    public interface IClienteService
    {
        Task<List<Cliente>> GetAll();
        Task AddCliente(Cliente cliente);
        Task UpdateCliente(Cliente cliente);
        Task DeleteCliente(Cliente cliente);
        Task MarcarCliente(Cliente cliente, int meses);
        Task CalculateFechaDePago(Cliente cliente, bool force = false, bool updatePago = false);
    }
}
