using System.Collections.Generic;
using System.Threading.Tasks;
using DateApp.Core.EntityModels;
using DateApp.Core.Helpers;

namespace DateApp.Core.Contracts.DAL
{
    public interface IMessageRepository : IBaseRepository
    {
        Task<Message> GetMessage(int id);
        Task<PagedList<Message>> GetMessagesForUser(MessageParams msgParams);
        Task<IEnumerable<Message>> GetMessageThread(int userId, int recipientId);
        Task<User> GetUser(int id);
    }
}
