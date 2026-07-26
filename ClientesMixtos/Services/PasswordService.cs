using ClientesMixtos.Repositories;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ClientesMixtos.Services
{
    public class PasswordService(PasswordRepository passwordRepository)
    {
        private readonly PasswordRepository _passwordRepository = passwordRepository;

        private static string EncryptPin(string pin)
        {
            byte[] hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(pin));

            return Convert.ToBase64String(hashBytes);
        }

        public Task<List<Models.Password>> GetAll()
        {
            return _passwordRepository.GetAll();
        }

        public async Task<bool> SavePassword(string pin, string user)
        {
            if (string.IsNullOrEmpty(pin)) return false;
            if (string.IsNullOrWhiteSpace(user)) return false;
            if (await _passwordRepository.FindUser(user) is not null) return false;

            var password = new Models.Password
            {
                EncryptedPin = EncryptPin(pin),
                Usuario = user
            };

            await _passwordRepository.InsertPassword(password);

            return true;
        }

        public async Task<bool> ExistsUser(string user)
        {
            return await _passwordRepository.FindUser(user) is not null;
        }

        public async Task<bool> VerifyPassword(string pin, string user)
        {
            var password = await _passwordRepository.FindUser(user);

            if (password is not null)
            {
                return password.EncryptedPin == EncryptPin(pin);
            }

            return false;
        }
    }
}
