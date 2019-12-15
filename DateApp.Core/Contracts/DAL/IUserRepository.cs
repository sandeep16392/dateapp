using System.Threading.Tasks;
using DateApp.Core.EntityModels;
using DateApp.Core.Helpers;

namespace DateApp.Core.Contracts.DAL
{
    public interface IUserRepository : IBaseRepository
    {
        Task<PagedList<User>> GetUsers(UserParams userParams);
        Task<User> GetUser(int id);
        Task<Like> GetLike(int userId, int recepientId);
    }
}
