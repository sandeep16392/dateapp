using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DateApp.Core.Contracts.DAL;
using DateApp.Core.EntityModels;
using DateApp.Core.Helpers;
using DateApp.DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace DateApp.DAL.Implementation
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        public UserRepository(DataContext context) : base(context)
        {
        }

        public async Task<PagedList<User>> GetUsers(UserParams userParams)
        {
            var users = Context.Users.Include(x => x.Photos).OrderByDescending(x => x.LastActive)
                .Where(x => x.Gender == userParams.Gender && x.Id != userParams.UserId);
            if (userParams.MaxAge != 99 || userParams.MinAge != 18)
            {
                var minDob = DateTime.Today.AddYears(-userParams.MaxAge - 1);
                var maxDob = DateTime.Today.AddYears(-userParams.MinAge);
                users = users.Where(x => x.DateOfBirth >= minDob && x.DateOfBirth <= maxDob);
            }


            if (userParams.Liker)
            {
                var userLikers = await UserLikes(userParams.UserId, userParams.Liker);
                users = users.Where(x => userLikers.Contains(x.Id));
            }

            if (userParams.Likee)
            {
                var userLikees = await UserLikes(userParams.UserId, userParams.Liker);
                users = users.Where(x => userLikees.Contains(x.Id));
            }

            if (!string.IsNullOrEmpty(userParams.OrderBy))
            {
                switch (userParams.OrderBy)
                {
                    case "created":
                        users = users.OrderByDescending(x => x.Created);
                        break;
                    default:
                        users = users.OrderByDescending(x => x.LastActive);
                        break;
                }
            }
            return await PagedList<User>.CreateAsync(users, userParams.PageNumber, userParams.PageSize);
        }

        private async Task<IEnumerable<int>> UserLikes(int id, bool likers)
        {
            var user = await Context.Users.Include(x => x.Likees).Include(x => x.Likers)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (likers)
            {
                return user.Likers.Where(x => x.LikeeId == id).Select(x => x.LikerId);
            }
            else
            {
                return user.Likees.Where(x => x.LikerId == id).Select(x => x.LikeeId);
            }
        }

        public async Task<User> GetUser(int id)
        {
            var user = await Context.Users.Include(x => x.Photos).FirstOrDefaultAsync(u => u.Id == id);

            return user;
        }

        public async Task<Like> GetLike(int userId, int recepientId)
        {
            return await Context.Likes.FirstOrDefaultAsync(x => x.LikerId == userId && x.LikeeId == recepientId);
        }
    }
}
