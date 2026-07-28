using ClientesMixtos.DB;
using ClientesMixtos.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClientesMixtos.Repos
{
    public class PasswordRepo(MongoContext context)
    {
        private readonly IMongoCollection<Password> _passwordCollection = context.GetCollection<Password>("passwords");

        public Task InsertPassword(Password password)
        {
            return _passwordCollection.InsertOneAsync(password);
        }

        public async Task<Password?> FindUser(string user)
        {
            return await _passwordCollection.Find(p => p.Usuario == user).FirstOrDefaultAsync();
        }

        public Task<List<Password>> GetAll()
        {
            return _passwordCollection.Find(_ => true).ToListAsync();
        }
    }
}
