using AutoMapper;
using DateApp.Core.Contracts.DAL;
using DateApp.Core.Contracts.Services;
using DateApp.Core.DataModels;
using DateApp.Core.EntityModels;
using DateApp.Core.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace DateApp.Business.Services
{
    public class UsersService : IUsersService
    {
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;

        public UsersService(IUserRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<UserDetailsDm> GetUser(int id)
        {
            var user = await _repository.GetUser(id);
            var userDm = _mapper.Map<UserDetailsDm>(user);
            return userDm;
        }

        public async Task<PaginateResponseDm> GetUsers(UserParams userParams, int currentUserId)
        {
            var currentUser = await _repository.GetUser(currentUserId);
            userParams.UserId = currentUserId;
            if (string.IsNullOrEmpty(userParams.Gender))
            {
                userParams.Gender = currentUser.Gender == "male" ? "female" : "male";
            }


            var users = await _repository.GetUsers(userParams);

            var userDms = _mapper.Map<IEnumerable<UserListDm>>(users);
            var paginatedResp = new PaginateResponseDm
            {
                Pagination = new PaginationDm(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages),
                Users = userDms
            };

            return paginatedResp;
        }

        public async Task<bool> LikeUser(int id, int recepientId)
        {
            var like = await _repository.GetLike(id, recepientId);

            if (like != null)
                throw new ArgumentException("You already Like this User");

            if (await _repository.GetUser(recepientId) == null)
                throw new NotSupportedException();

            like = new Like
            {
                LikerId = id,
                LikeeId = recepientId
            };

            _repository.Add(like);

            if (await _repository.SaveAll())
                return true;

            return false;
        }

        public async Task<bool> UpdateUser(int id, UserUpdateDm userForUpdate)
        {
            var userFromRepo = await _repository.GetUser(id);
            var userToSave = _mapper.Map(userForUpdate, userFromRepo);

            if (await _repository.SaveAll())
                return true;

            return false;
        }
    }
}
