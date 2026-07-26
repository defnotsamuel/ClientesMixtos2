using System.Globalization;
using ClientesMixtos.Repositories;
using ClientesMixtos.Models;
using System.Threading.Tasks;

namespace ClientesMixtos.Services
{
    public class ClienteServiceN(ClienteRepository repository)
    {
        private readonly ClienteRepository _repository = repository;

        public async Task<List<Cliente>> GetAll()
        {
            var clientes = await _repository.GetAll();

            foreach (var cliente in clientes)
            {

                cliente.State.FechaDeCompra = DateUtils.Utils.ParseDate(cliente.FechaDeCompra);
                cliente.State.FechaMarcada = DateUtils.Utils.ParseDate(cliente.FechaMarcada);
                cliente.State.FechaDePago = DateUtils.Utils.ParseDate(cliente.FechaDePago);

                ActualizarEstado(cliente);

                if (cliente.State.FechaDePago == null)
                {
                    DateUtils.Utils.CalculateFechaDePago(cliente);
                }

                cliente.State.Pendiente = cliente.State.FechaDePago <= DateTime.Now;
                MarcadoEsteMes(cliente);
            }

            return clientes;
        }

        public async Task UpdateCliente(Cliente cliente)
        {

            ActualizarEstado(cliente);

            if (cliente.State.FechaDePago == null)
            {
                DateUtils.Utils.CalculateFechaDePago(cliente);
            }

            cliente.State.Pendiente = cliente.State.FechaDePago <= DateTime.Now;
            MarcadoEsteMes(cliente);

            await _repository.UpdateCliente(cliente);
        }


        public async Task DeleteCliente(Cliente cliente)
        {
            await _repository.DeleteCliente(cliente);
        }

        public async Task AddCliente(Cliente cliente)
        {
            ActualizarEstado(cliente);

            if (cliente.State.FechaDePago == null)
            {
                DateUtils.Utils.CalculateFechaDePago(cliente);
            }

            cliente.State.Pendiente = cliente.State.FechaDePago <= DateTime.Now;
            MarcadoEsteMes(cliente);

           await _repository.InsertCliente(cliente);
        }


        public async Task MarcarCliente(Cliente cliente, int meses = 1)
        {

            if (cliente.State.FechaDePago == null) return;

            cliente.FechaMarcada = DateTime.Today.ToString("dd/MM/yyyy");

            var fechaDePago = (DateTime)cliente.State.FechaDePago;
            cliente.FechaDePago = fechaDePago.AddMonths(meses).ToString("dd/MM/yyyy");

            ActualizarEstado(cliente);
            MarcadoEsteMes(cliente);

            await _repository.UpdateFechaMarcada(cliente);
            await _repository.UpdateFechaPago(cliente);
        }


        private static void MarcadoEsteMes(Cliente cliente)
        {
            var fechaMarcada = cliente.State.FechaMarcada;

            cliente.State.MarcadoEsteMes = fechaMarcada?.Month == DateTime.Today.Month &&
                fechaMarcada?.Year == DateTime.Today.Year;
        }

        private static void ActualizarEstado(Cliente cliente)
        {
            cliente.State.FechaDeCompra = DateUtils.Utils.ParseDate(cliente.FechaDeCompra);
            cliente.State.FechaMarcada = DateUtils.Utils.ParseDate(cliente.FechaMarcada);
            cliente.State.FechaDePago = DateUtils.Utils.ParseDate(cliente.FechaDePago);
        }
    }
}
