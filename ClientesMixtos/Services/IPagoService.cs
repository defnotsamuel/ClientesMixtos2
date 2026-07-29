using ClientesMixtos.Models;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClientesMixtos.Services
{
    public interface IPagoService
    {
        Task<List<Pago>> GetHistorial(ObjectId clienteId);
        Task<bool> ExistePago(ObjectId clienteId, DateTime fechaPagada);
        Task<Pago?> RegistrarPago(ObjectId clienteId, DateTime fechaPagada, DateTime? fechaMarcado = null);
        Task CrearHistorialInicial(Cliente cliente);
        Task EliminarPago(ObjectId id);
        Task CrearPagoDesdeFechaMarcada(Cliente cliente);
    }
}
