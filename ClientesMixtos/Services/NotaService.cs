using ClientesMixtos.DateUtils;
using ClientesMixtos.Models;
using ClientesMixtos.Repos;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClientesMixtos.Services
{
    public class NotaService : INotaService
    {
        private readonly INotaRepo _notaRepo;
        private readonly IDateUtils _dateUtils;

        public NotaService(INotaRepo notaRepo, IDateUtils dateUtils)
        {
            _notaRepo = notaRepo;
            _dateUtils = dateUtils;
        }

        public async Task<List<Nota>> FromClient(Cliente cliente)
        {
            var notas = await _notaRepo.GetByClienteId(cliente.ClienteId);

            foreach (var nota in notas)
            {
                nota.State.FechaCreacion = _dateUtils.ParseDate(nota.FechaCreacion);
            }

            return notas;
        }

        public Task Insert(Nota nota, Cliente cliente)
        {
            nota.ClienteId = cliente.ClienteId;
            return _notaRepo.InsertNota(nota);
        }

        public Task Delete(Nota nota) => _notaRepo.DeleteNota(nota);

        public Task DeleteByClienteId(ObjectId clienteId) => _notaRepo.DeleteByClienteId(clienteId);

        public Task Update(Nota nota) => _notaRepo.UpdateNota(nota);
    }
}
