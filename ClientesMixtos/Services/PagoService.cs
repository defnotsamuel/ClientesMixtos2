using ClientesMixtos.Models;
using ClientesMixtos.Repos;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClientesMixtos.Services
{
    public class PagoService(PagoRepo repository)
    {
        private readonly PagoRepo _repository = repository;

        public Task<List<Pago>> GetHistorial(string clienteId)
        {
            return _repository.GetByClienteId(clienteId);
        }

        public async Task<bool> ExistePago(string clienteId, DateTime fechaPagada)
        {
            return await _repository.GetByFecha(clienteId, fechaPagada) is not null;
        }

        public async Task<Pago?> RegistrarPago(
            string clienteId,
            DateTime fechaPagada,
            DateTime? fechaMarcado = null)
        {
            if (await ExistePago(clienteId, fechaPagada))
                return null;

            var pago = new Pago
            {
                ClienteId = clienteId,
                FechaPagada = fechaPagada,
                FechaMarcado = fechaMarcado
            };

            await _repository.Insert(pago);
            return pago;
        }

        public async Task CrearHistorialInicial(Cliente cliente)
        {
            if (cliente.State.FechaDeCompra is null ||
                cliente.State.FechaDePago is null)
                return;

            var pagos = new List<Pago>();

            DateTime primerPago = DateUtils.Utils.CrearFechaValida(
                cliente.State.FechaDeCompra.Value,
                cliente.State.FechaDeCompra.Value.Day);

            for (DateTime pago = cliente.State.FechaDePago.Value.AddMonths(-1);
                 pago >= primerPago;
                 pago = pago.AddMonths(-1))
            {
                pagos.Add(new Pago
                {
                    ClienteId = cliente.ClienteId,
                    FechaPagada = pago,
                    FechaMarcado = pago
                });
            }

            if (pagos.Count > 0)
                await _repository.InsertMany(pagos);
        }

        public Task EliminarPago(ObjectId id)
        {
            return _repository.Delete(id);
        }

        public async Task CrearPagoDesdeFechaMarcada(Cliente cliente)
        {
            if (cliente.State.FechaDeCompra is null ||
                cliente.State.FechaMarcada is null)
                return;

            DateTime fechaCompra = cliente.State.FechaDeCompra.Value.Date;
            DateTime fechaMarcada = cliente.State.FechaMarcada.Value.Date;

            int diaPago = Math.Min(
                fechaCompra.Day,
                DateTime.DaysInMonth(fechaMarcada.Year, fechaMarcada.Month));

            DateTime fechaPagada = new(
                fechaMarcada.Year,
                fechaMarcada.Month,
                diaPago);

            var existente = await _repository.GetByFecha(
                cliente.ClienteId,
                fechaPagada);

            if (existente is not null)
                return;

            await _repository.Insert(new Pago
            {
                ClienteId = cliente.ClienteId,
                FechaPagada = fechaPagada,
                FechaMarcado = fechaMarcada
            });
        }
    }
}
