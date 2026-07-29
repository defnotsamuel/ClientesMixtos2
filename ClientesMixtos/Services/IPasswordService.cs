using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClientesMixtos.Services
{
    public interface IPasswordService
    {
        Task<List<Models.Password>> GetAll();
        Task<bool> HasUsers();
        Task<bool> SavePassword(string pin, string user);
        Task<bool> ExistsUser(string user);
        Task<bool> VerifyPassword(string pin, string user);
    }
}
