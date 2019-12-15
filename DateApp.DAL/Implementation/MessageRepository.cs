using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DateApp.Core.Contracts.DAL;
using DateApp.Core.EntityModels;
using DateApp.Core.Helpers;
using DateApp.DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace DateApp.DAL.Implementation
{
    public class MessageRepository : BaseRepository, IMessageRepository
    {
        public MessageRepository(DataContext context) : base(context)
        {
        }

        public async Task<Message> GetMessage(int id)
        {
            var messages = await Context.Messages.FirstOrDefaultAsync(x => x.Id == id);
            return messages;
        }

        public async Task<PagedList<Message>> GetMessagesForUser(MessageParams msgParams)
        {
            var messages = Context.Messages
                .Include(x => x.Sender).ThenInclude(x => x.Photos)
                .Include(x => x.Recipient).ThenInclude(x => x.Photos)
                .AsQueryable();

            switch (msgParams.MessageContainer)
            {
                case "Inbox":
                    messages = messages.Where(x => x.RecipientId == msgParams.UserId && !x.RecipientDeleted);
                    break;
                case "Outbox":
                    messages = messages.Where(x => x.SenderId == msgParams.UserId && !x.SenderDeleted);
                    break;
                default:
                    messages = messages.Where(x => x.RecipientId == msgParams.UserId && !x.IsRead && !x.RecipientDeleted);
                    break;
            }

            messages = messages.OrderByDescending(x => x.MessageSent);
            return await PagedList<Message>.CreateAsync(messages, msgParams.PageNumber, msgParams.PageSize);
        }

        public async Task<IEnumerable<Message>> GetMessageThread(int userId, int recipientId)
        {
            var messages = await Context.Messages
                .Include(x => x.Sender).ThenInclude(x => x.Photos)
                .Include(x => x.Recipient).ThenInclude(x => x.Photos)
                .Where(x => x.RecipientId == userId && !x.RecipientDeleted && x.SenderId == recipientId ||
                            x.RecipientId == recipientId && x.SenderId == userId && !x.SenderDeleted)
                .OrderByDescending(x => x.MessageSent).ToListAsync();

            return messages;
        }

        public async Task<User> GetUser(int id)
        {
            var user = await Context.Users.Include(x => x.Photos).FirstOrDefaultAsync(u => u.Id == id);

            return user;
        }
    }
}
