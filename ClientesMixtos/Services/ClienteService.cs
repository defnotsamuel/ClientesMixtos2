using ClientesMixtos.DateUtils;
using ClientesMixtos.Models;
using ClientesMixtos.Repos;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ClientesMixtos.Services
{
    public class ClienteService : IClienteService
    {
        private readonly IClienteRepo _clienteRepo;
        private readonly IPagoService _pagoService;
        private readonly IDateUtils _dateUtils;
        private readonly ILogger<ClienteService> _logger;

        public ClienteService(IClienteRepo clienteRepo, IPagoService pagoService, IDateUtils dateUtils, ILogger<ClienteService> logger)
        {
            _clienteRepo = clienteRepo;
            _pagoService = pagoService;
            _dateUtils = dateUtils;
            _logger = logger;
        }

        public Task<List<Cliente>> GetAll()
        {
            return LoadAsync();
        }

        private async Task<List<Cliente>> LoadAsync()
        {
            var clientes = await _clienteRepo.GetAll();

            var startWatch = Stopwatch.StartNew();

            foreach (var cliente in clientes)
            {

                LoadDates(cliente);

                await _pagoService.CrearPagoDesdeFechaMarcada(cliente);
                await CalculateFechaDePago(cliente, updatePago: true);
                await LoadStates(cliente);
            }

            startWatch.Stop();

            var end = startWatch.ElapsedMilliseconds / 1000;

            _logger.LogInformation("Tiempo transcurrido en LoadAsync: {end}s", end);

            return clientes;
        }

        public async Task AddCliente(Cliente cliente)
        {
            await _clienteRepo.InsertCliente(cliente);

            LoadDates(cliente);

            await CalculateFechaDePago(cliente);
            await _pagoService.CrearHistorialInicial(cliente);

            await LoadStates(cliente);
        }

        public async Task UpdateCliente(Cliente cliente)
        {
            LoadDates(cliente);
            await CalculateFechaDePago(cliente);

            await _clienteRepo.UpdateCliente(cliente);
            await LoadStates(cliente);
        }

        public async Task DeleteCliente(Cliente cliente)
        {
            await _clienteRepo.DeleteCliente(cliente);
        }

        public async Task MarcarCliente(Cliente cliente, int meses)
        {
            if (!cliente.State.FechaDePago.HasValue) return;

            var fechaMarcada = DateTime.Now.Date;

            for (int i = 1; i <= meses; i++)
            {
                await _pagoService.RegistrarPago(cliente.ClienteId,
                    cliente.State.FechaDePago.Value,
                    fechaMarcada);

                cliente.State.FechaDePago = cliente.State.FechaDePago.Value.AddMonths(1);
            }

            cliente.State.FechaMarcada = fechaMarcada;

            cliente.FechaMarcada = fechaMarcada.ToString("dd/MM/yyyy");
            cliente.FechaDePago = cliente.State.FechaDePago.Value.ToString("dd/MM/yyyy");

            await _clienteRepo.UpdateFechaMarcada(cliente);
            await _clienteRepo.UpdateFechaPago(cliente);

            await LoadStates(cliente);
        }

        private async Task LoadStates(Cliente cliente, List<Pago>? pagosCliente = null)
        {
            pagosCliente ??= await _pagoService.GetHistorial(cliente.ClienteId);

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

            cliente.State.MesesAtrasado = CalculateMonthsLate(
                cliente.State.FechaDePago,
                hoy);
        }

        private void LoadDates(Cliente cliente)
        {
            cliente.State.FechaDeCompra = _dateUtils.ParseDate(cliente.FechaDeCompra);
            cliente.State.FechaMarcada = _dateUtils.ParseDate(cliente.FechaMarcada);
            cliente.State.FechaDePago = _dateUtils.ParseDate(cliente.FechaDePago);
            cliente.State.FechaVence = _dateUtils.ParseDate(cliente.FechaVence);
        }

        public async Task CalculateFechaDePago(Cliente cliente, bool force = false, bool updatePago = false)
        {
            if (cliente.State.FechaDeCompra is null)
                return;

            if (!force && cliente.State.FechaDePago is not null)
                return;

            if (!force && cliente.State.FechaMarcada is not null)
                return;

            DateTime fechaPago = _dateUtils.GetNextPaymentDate(cliente.State.FechaDeCompra.Value);
            DateTime primeraFechaPago = new(2026, 7, fechaPago.Day);

            var fechaPagada = fechaPago.AddMonths(-1);

            for (; ; )
            {
                bool pagoEsteMes = await _pagoService.ExistePago(cliente.ClienteId, fechaPagada);
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
                await _clienteRepo.UpdateFechaPago(cliente);
        }

        private static int CalculateMonthsLate(
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
