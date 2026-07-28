using ClientesMixtos.Models;
using ClientesMixtos.Repos;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace ClientesMixtos.Services
{
    public class NotaService(NotaRepo notaRepository)
    {
        private readonly NotaRepo _notaRepository = notaRepository;

        public async Task<List<Nota>> FromClient(Cliente cliente)
        {
            var notas = await _notaRepository.GetByClienteId(cliente.ClienteId);

            foreach (var nota in notas)
            {
                nota.State.FechaCreacion = ParseDate(nota.FechaCreacion);
            }

            return notas;
        }

        public Task Insert(Nota nota, Cliente cliente)
        {
            nota.ClienteId = cliente.ClienteId;
            return _notaRepository.InsertNota(nota);
        }

        public Task Delete(Nota nota) => _notaRepository.DeleteNota(nota);

        public Task DeleteByClienteId(string clienteId) => _notaRepository.DeleteByClienteId(clienteId);

        public Task Update(Nota nota) => _notaRepository.UpdateNota(nota);

        private static DateTime? ParseDate(string date)
        {
            bool success = DateTime.TryParseExact(
                    date,
                    "dd/MM/yyyy",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime fechaPago);

            return success ? fechaPago : null;
        }
    }
}
