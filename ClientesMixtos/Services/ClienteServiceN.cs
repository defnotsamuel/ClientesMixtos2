using ClientesMixtos.Models;
using ClientesMixtos.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClientesMixtos.Services
{
    public class ClienteService(ClienteRepository clienteRepository, PagosClienteRepository pagosClienteRepository)
    {

        private readonly ClienteRepository _clienteRepository = clienteRepository;
        private readonly PagosClienteRepository _pagosClienteRepository = pagosClienteRepository;

        public Task<List<Cliente>> GetAll()
        {
            return LoadAsync();
        }

        private async Task<List<Cliente>> LoadAsync()
        {
            var clientes = await _clienteRepository.GetAll();

            foreach (var cliente in clientes)
            {
                CargarFechas(cliente);
                DateUtils.Utils.CalculateFechaDePago(cliente);

                var pagosCliente = await CargarPagosCliente(cliente);
                await CargarEstados(cliente, pagosCliente);
            }

            return clientes;
        }

        private async Task<PagosCliente?> CargarPagosCliente(Cliente cliente) {
            var check = await _pagosClienteRepository.GetByClienteId(cliente.ClienteId);
            if (check is not null) return check;

            // Necesitamos crear el historial para el cliente primero
            // Evitaremos actualizar si aun no hay fecha de pago
            if (cliente.State.FechaDePago is null) return null;

            var pagosCliente = new PagosCliente()
            {
                ClienteId = cliente.ClienteId
            };

            var fechaPago = cliente.State.FechaDePago.Value;
            var fechaCompra = cliente.State.FechaDeCompra.Value;
            var primerPago = new DateTime(fechaCompra.Year, fechaCompra.Month, fechaPago.Day);

            if (fechaPago != primerPago)
            {
                // Si no son iguales quiere decir que el cliente ya ha pagado
                // Entonces agregemos los meses faltantes a la lista

                var actual = fechaPago;
                while (actual > primerPago)
                {
                    actual = actual.AddMonths(-1);

                    pagosCliente.Pagos.Add(new Pago()
                    {
                        FechaPagada = actual,

                        // Probablemente se pierda informacion
                        // Sobre la fecha marcada ahora
                        FechaMarcado = cliente.State.FechaMarcada ?? DateTime.MinValue
                    });
                }
            }

            cliente.State.FechasPagadas = pagosCliente.Pagos;
            await _pagosClienteRepository.Insert(pagosCliente);
            return pagosCliente;
        }

        public async Task AddCliente(Cliente cliente)
        {
            CargarFechas(cliente);
            DateUtils.Utils.CalculateFechaDePago(cliente);

            await _clienteRepository.InsertCliente(cliente);
            await CargarPagosCliente(cliente);
            await CargarEstados(cliente);
        }

        public async Task UpdateCliente(Cliente cliente)
        {
            CargarFechas(cliente);
            DateUtils.Utils.CalculateFechaDePago(cliente);

            await _clienteRepository.UpdateCliente(cliente);
            await CargarEstados(cliente);
        }

        public async Task DeleteCliente(Cliente cliente)
        {
            await _clienteRepository.DeleteCliente(cliente);
        }

        public async Task MarcarCliente(Cliente cliente, int meses)
        {
            if (cliente.State.FechaDePago is null) return;

            var pagosCliente = await _pagosClienteRepository.GetByClienteId(cliente.ClienteId);
            if (pagosCliente is null) return;

            for (int i = 1; i <= meses; i++)
            {
                await MarcarUnMes(cliente, pagosCliente);
            }

            cliente.FechaDePago = cliente.State.FechaDePago?.ToString("dd/MM/yyyy");
            cliente.FechaMarcada = cliente.State.FechaMarcada?.ToString("dd/MM/yyyy");

            await CargarEstados(cliente);

            await _clienteRepository.UpdateCliente(cliente);
        }

        private async Task MarcarUnMes(Cliente cliente, PagosCliente pagosCliente)
        {
            cliente.State.FechaMarcada = DateTime.Now.Date;
            
            var fechaPago = cliente.State.FechaDePago ?? DateTime.MinValue;
            cliente.State.FechaDePago = fechaPago.AddMonths(1);

            var pago = new Pago
            {
                FechaMarcado = cliente.State.FechaMarcada.Value,
                FechaPagada = fechaPago,
            };

            pagosCliente.Pagos.Add(pago);

            await _pagosClienteRepository.Update(pagosCliente);
        }

        private async Task CargarEstados(Cliente cliente, PagosCliente? pagosCliente = null)
        {
            pagosCliente ??= await _pagosClienteRepository.GetByClienteId(cliente.ClienteId);

            // No podemos hacer nada
            if (pagosCliente is null) return;

            // Pendiente sera verdadero si la fecha de pago ya
            // es la misma fecha que la actual o si ya se paso
            // Y sera falso si aun falta

            var fechaPago = cliente.State.FechaDePago;
            var hoy = DateTime.Now.Date;

            if (fechaPago?.Month == cliente.State.FechaMarcada?.Month && fechaPago?.Year == cliente.State.FechaMarcada?.Year)
            {
                cliente.State.FechaDePago = fechaPago?.AddMonths(1);
                cliente.FechaDePago = cliente.State.FechaDePago?.ToString("dd/MM/yyy");

                pagosCliente.Pagos.Add(new Pago
                {
                    FechaMarcado = cliente.State.FechaMarcada.Value,
                    FechaPagada = cliente.State.FechaDePago.Value.AddMonths(-1)
                });
            }

            cliente.State.Pendiente = (fechaPago?.Month == hoy.Month && fechaPago?.Year == hoy.Year) 
                                    && hoy.Day >= fechaPago?.Day;


            cliente.State.PagoEsteMes = pagosCliente.Pagos.Any(p => p.FechaPagada.Year == hoy.Year &&
                                                                    p.FechaPagada.Month == hoy.Month);

            cliente.State.MarcadoEsteMes = (cliente.State.FechaMarcada?.Month == hoy.Month
                                        && cliente.State.FechaMarcada?.Year == hoy.Year) && cliente.State.PagoEsteMes;

            if (cliente.State.FechaDeCompra.HasValue)
            {
                var fechaCompra = cliente.State.FechaDeCompra.Value;
                int mesesEsperados;
                int mesInicio = 1; // enero

                if (fechaCompra.Year == hoy.Year && fechaCompra.Month > 1)
                    mesInicio = fechaCompra.Month;

                if (fechaCompra.Year == hoy.Year)
                    mesesEsperados = hoy.Month - mesInicio + 1;
                else
                    mesesEsperados = hoy.Month - mesInicio + 1;

                int mesesPagados = pagosCliente.Pagos
                    .Select(p => new DateTime(p.FechaPagada.Year, p.FechaPagada.Month, 1))
                    .Distinct()
                    .Count();

                cliente.State.MesesAtrasado = Math.Max(0, mesesEsperados - mesesPagados);
            }
        }

        private static void CargarFechas(Cliente cliente)
        {
            cliente.State.FechaDeCompra = DateUtils.Utils.ParseDate(cliente.FechaDeCompra);
            cliente.State.FechaMarcada = DateUtils.Utils.ParseDate(cliente.FechaMarcada);
            cliente.State.FechaDePago = DateUtils.Utils.ParseDate(cliente.FechaDePago);
            cliente.State.FechaVence = DateUtils.Utils.ParseDate(cliente.FechaVence);
        }
    }
}
