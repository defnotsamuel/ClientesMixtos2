using ClientesMixtos.Models;
using ClientesMixtos.Repos;
using ClientesMixtos.DateUtils;
using System.Collections.Generic;
using System.Text;

namespace ClientesMixtos.Services
{
    public class ClienteService(ClienteRepo clienteRepository, PagoService pagoService)
    {

        private readonly ClienteRepo clienteRepository = clienteRepository;
        private readonly PagoService pagoService = pagoService;

        public Task<List<Cliente>> GetAll()
        {
            return LoadAsync();
        }

        private async Task<List<Cliente>> LoadAsync()
        {
            var clientes = await clienteRepository.GetAll();

            foreach (var cliente in clientes)
            {
                CargarFechas(cliente);

                await pagoService.CrearPagoDesdeFechaMarcada(cliente);
                await CalculateFechaDePago(cliente, updatePago: true);
                await CargarEstados(cliente);
            }

            return clientes;
        }

        public async Task AddCliente(Cliente cliente)
        {
            await clienteRepository.InsertCliente(cliente);

            CargarFechas(cliente);

            await CalculateFechaDePago(cliente);
            await pagoService.CrearHistorialInicial(cliente);

            await CargarEstados(cliente);
        }

        public async Task UpdateCliente(Cliente cliente)
        {
            CargarFechas(cliente);
            await CalculateFechaDePago(cliente);

            await clienteRepository.UpdateCliente(cliente);
            await CargarEstados(cliente);
        }

        public async Task DeleteCliente(Cliente cliente)
        {
            await clienteRepository.DeleteCliente(cliente);
        }

        public async Task MarcarCliente(Cliente cliente, int meses)
        {
            if (!cliente.State.FechaDePago.HasValue) return;

            var fechaMarcada = DateTime.Now.Date;

            for (int i = 1; i <= meses; i++)
            {
                await pagoService.RegistrarPago(cliente.ClienteId,
                    cliente.State.FechaDePago.Value, 
                    fechaMarcada);

                cliente.State.FechaDePago = cliente.State.FechaDePago.Value.AddMonths(1);
            }

            cliente.State.FechaMarcada = fechaMarcada;

            cliente.FechaMarcada = fechaMarcada.ToString("dd/MM/yyyy");
            cliente.FechaDePago = cliente.State.FechaDePago.Value.ToString("dd/MM/yyyy");

            await clienteRepository.UpdateFechaMarcada(cliente);
            await clienteRepository.UpdateFechaPago(cliente);

            await CargarEstados(cliente);
        }


        private async Task CargarEstados(Cliente cliente, List<Pago>? pagosCliente = null)
        {
            pagosCliente ??= await pagoService.GetHistorial(cliente.ClienteId);

            if (pagosCliente is null)
                return;

            DateTime hoy = DateTime.Today;
            DateTime? fechaPago = cliente.State.FechaDePago;
            DateTime? fechaMarcada = cliente.State.FechaMarcada;

            cliente.State.Pendiente =
                fechaPago.HasValue &&
                fechaPago.Value.Date <= hoy;

            cliente.State.PagoEsteMes = pagosCliente.Any(p =>
                p.FechaPagada.Year == hoy.Year &&
                p.FechaPagada.Month == hoy.Month);

            cliente.State.MarcadoEsteMes =
                cliente.State.PagoEsteMes &&
                fechaMarcada?.Year == hoy.Year &&
                fechaMarcada?.Month == hoy.Month;

            cliente.State.MesesAtrasado = CalcularMesesAtrasado(
                cliente.State.FechaDePago,
                hoy);
        }

        private static void CargarFechas(Cliente cliente)
        {
            cliente.State.FechaDeCompra = Utils.ParseDate(cliente.FechaDeCompra);
            cliente.State.FechaMarcada = Utils.ParseDate(cliente.FechaMarcada);
            cliente.State.FechaDePago = Utils.ParseDate(cliente.FechaDePago);
            cliente.State.FechaVence = Utils.ParseDate(cliente.FechaVence);
        }

        public async Task CalculateFechaDePago(Cliente cliente, bool force = false, bool updatePago = false)
        {
            if (cliente.State.FechaDeCompra is null)
                return;

            if (!force && cliente.State.FechaDePago is not null)
                return;

            if (!force && cliente.State.FechaMarcada is not null) 
                return;

            DateTime fechaPago = Utils.ObtenerProximaFechaPago(cliente.State.FechaDeCompra.Value);
            DateTime primeraFechaPago = new(2026, 7, fechaPago.Day);

            var fechaPagada = fechaPago.AddMonths(-1);

            for (; ; )
            {
                bool pagoEsteMes = await pagoService.ExistePago(cliente.ClienteId, fechaPagada);
                if (pagoEsteMes)
                {
                    fechaPago = fechaPagada.AddMonths(1);
                    break;
                }

                if (primeraFechaPago >= fechaPagada)
                {
                    fechaPago = primeraFechaPago;
                    break;
                }

                fechaPagada = fechaPago.AddMonths(-1);
            }

            cliente.State.FechaDePago = fechaPago;
            cliente.FechaDePago = fechaPago.ToString("dd/MM/yyyy");

            if (updatePago)
                await clienteRepository.UpdateFechaPago(cliente);
        }

        private static int CalcularMesesAtrasado(
            DateTime? fechaPago,
            DateTime hoy)
        {
            if (!fechaPago.HasValue)
                return 0;

            fechaPago = fechaPago.Value.Date;
            hoy = hoy.Date;

            if (hoy < fechaPago)
                return 0;

            int meses =
                (hoy.Year - fechaPago.Value.Year) * 12 +
                (hoy.Month - fechaPago.Value.Month);

            return meses;
        }
    }
}
