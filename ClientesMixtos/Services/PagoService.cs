using ClientesMixtos.Models;
using ClientesMixtos.Repos;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ClientesMixtos.Services
{
    public class PagoService : IPagoService
    {
        private readonly IPagoRepo _repo;

        public PagoService(IPagoRepo repo)
        {
            _repo = repo;
        }

        public Task<List<Pago>> GetHistorial(ObjectId clienteId)
        {
            return _repo.GetByClienteId(clienteId);
        }

        public async Task<bool> ExistePago(ObjectId clienteId, DateTime fechaPagada)
        {
            return await _repo.GetByFecha(clienteId, fechaPagada) is not null;
        }

        public async Task<Pago?> RegistrarPago(
            ObjectId clienteId,
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

            await _repo.Insert(pago);
            return pago;
        }

        public async Task CrearHistorialInicial(Cliente cliente)
        {
            if (cliente.State.FechaDeCompra is null ||
                cliente.State.FechaDePago is null)
                return;

            var pagos = new List<Pago>();

            DateTime hoy = DateTime.Today;
            int diaPago = cliente.State.FechaDeCompra.Value.Day;

            DateTime primerPago = new(
                hoy.Year,
                7,
                Math.Min(diaPago, DateTime.DaysInMonth(hoy.Year, 7)));

            DateTime ultimoPago = cliente.State.FechaDePago.Value.AddMonths(-1);

            if (ultimoPago < primerPago)
                return;

            for (DateTime pago = ultimoPago;
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
            {
                pagos.Reverse();
                await _repo.InsertMany(pagos);
            }
        }

        public Task EliminarPago(ObjectId id)
        {
            return _repo.Delete(id);
        }

        public async Task CrearPagoDesdeFechaMarcada(Cliente cliente)
        {
            if (cliente.State.FechaDeCompra is null ||
                cliente.State.FechaMarcada is null)
                return;

            var stopWatch = Stopwatch.StartNew();

            DateTime fechaCompra = cliente.State.FechaDeCompra.Value.Date;
            DateTime fechaMarcada = cliente.State.FechaMarcada.Value.Date;

            int diaPago = Math.Min(
                fechaCompra.Day,
                DateTime.DaysInMonth(fechaMarcada.Year, fechaMarcada.Month));

            DateTime fechaPagada = new(
                fechaMarcada.Year,
                fechaMarcada.Month,
                diaPago);

            var existente = await _repo.GetByFecha(
                cliente.ClienteId,
                fechaPagada);

            if (existente is not null)
                return;

            await _repo.Insert(new Pago
            {
                ClienteId = cliente.ClienteId,
                FechaPagada = fechaPagada,
                FechaMarcado = fechaMarcada
            });

            stopWatch.Stop();
            var end = stopWatch.ElapsedMilliseconds;


        }
    }
}
