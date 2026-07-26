using ClientesMixtos.DB;
using ClientesMixtos.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClientesMixtos.Repositories
{
    public class ClienteRepository(MongoContext context)
    {
        private readonly IMongoCollection<Cliente> _collection = context.GetCollection<Cliente>("clientes");

        public Task<List<Cliente>> GetAll()
        {
            return _collection.Find(_ => true).ToListAsync();
        }

        public Task UpdateFechaPago(Cliente cliente)
        {
            var filter = Builders<Cliente>.Filter.Eq(c => c.ClienteId, cliente.ClienteId);
            var update = Builders<Cliente>.Update.Set(c => c.FechaDePago, cliente.FechaDePago);
            return _collection.UpdateOneAsync(filter, update);
        }

        public Task UpdateFechaMarcada(Cliente cliente)
        {
            var filter = Builders<Cliente>.Filter.Eq(c => c.ClienteId, cliente.ClienteId);
            var update = Builders<Cliente>.Update.Set(c => c.FechaMarcada, cliente.FechaMarcada);
            return _collection.UpdateOneAsync(filter, update);
        }

        public Task InsertCliente(Cliente cliente)
        {
            return _collection.InsertOneAsync(cliente);
        }

        public Task DeleteCliente(Cliente cliente)
        {
            var filter = Builders<Cliente>.Filter.Eq(c => c.ClienteId, cliente.ClienteId);
            return _collection.DeleteOneAsync(filter);
        }

        public Task UpdateCliente(Cliente cliente)
        {
            var filter = Builders<Cliente>.Filter.Eq(c => c.ClienteId, cliente.ClienteId);
            var update = Builders<Cliente>.Update
                .Set(c => c.Lote, cliente.Lote)
                .Set(c => c.Libro1, cliente.Libro1)
                .Set(c => c.Libro2, cliente.Libro2)
                .Set(c => c.Nombre, cliente.Nombre)
                .Set(c => c.Vehiculo, cliente.Vehiculo)
                .Set(c => c.Telefono, cliente.Telefono)
                .Set(c => c.Ciudad, cliente.Ciudad)
                .Set(c => c.Placa, cliente.Placa)
                .Set(c => c.FechaDeCompra, cliente.FechaDeCompra)
                .Set(c => c.FechaDePago, cliente.FechaDePago)
                .Set(c => c.FechaMarcada, cliente.FechaMarcada)
                .Set(c => c.FechaVence, cliente.FechaVence)
                .Set(c => c.Recuperado, cliente.Recuperado)
                .Set(c => c.Perdido, cliente.Perdido)
                .Set(c => c.Refrenda, cliente.Refrenda);

            return _collection.UpdateOneAsync(filter, update);
        }
    }
}
