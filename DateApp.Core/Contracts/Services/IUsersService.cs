using DateApp.Core.DataModels;
using DateApp.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DateApp.Core.Contracts.Services
{
    public interface IUsersService
    {
        public Task<PaginateResponseDm> GetUsers(UserParams userParams, int currentUserId);

        public Task<bool> LikeUser(int id, int recepientId);
        public Task<UserDetailsDm> GetUser(int id);
        public Task<bool> UpdateUser(int id, UserUpdateDm userForUpdate);
    }
}
