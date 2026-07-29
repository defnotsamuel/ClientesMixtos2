using ClientesMixtos.Repos;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ClientesMixtos.Services
{
    public class PasswordService : IPasswordService
    {
        private readonly IPasswordRepo _passwordRepo;

        public PasswordService(IPasswordRepo passwordRepo)
        {
            _passwordRepo = passwordRepo;
        }

        private static string EncryptPin(string pin)
        {
            byte[] hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(pin));
            return Convert.ToBase64String(hashBytes);
        }

        public Task<List<Models.Password>> GetAll()
        {
            return _passwordRepo.GetAll();
        }

        public async Task<bool> HasUsers()
        {
            var passwords = await GetAll();
            return passwords.Count > 0;
        }

        public async Task<bool> SavePassword(string pin, string user)
        {
            if (string.IsNullOrEmpty(pin)) return false;
            if (string.IsNullOrWhiteSpace(user)) return false;
            if (await _passwordRepo.FindUser(user) is not null) return false;

            var password = new Models.Password
            {
                EncryptedPin = EncryptPin(pin),
                Usuario = user
            };

            await _passwordRepo.InsertPassword(password);
            return true;
        }

        public async Task<bool> ExistsUser(string user)
        {
            return await _passwordRepo.FindUser(user) is not null;
        }

        public async Task<bool> VerifyPassword(string pin, string user)
        {
            var password = await _passwordRepo.FindUser(user);

            if (password is not null)
            {
                return password.EncryptedPin == EncryptPin(pin);
            }

            return false;
        }
    }
}
