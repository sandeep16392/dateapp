using System.Threading.Tasks;
using DateApp.Core.EntityModels;

namespace DateApp.Core.Contracts.DAL
{
    public interface IAuthRepository
    {
        Task<User> Register(User user, string password);
        Task<User> Login(string username, string password);
        Task<bool> UserExists(string username);
    }
}
