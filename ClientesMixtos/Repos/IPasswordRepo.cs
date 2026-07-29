using ClientesMixtos.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClientesMixtos.Repos
{
    public interface IPasswordRepo
    {
        Task InsertPassword(Password password);
        Task<Password?> FindUser(string user);
        Task<List<Password>> GetAll();
    }
}
