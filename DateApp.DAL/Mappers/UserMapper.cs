using System.Collections.Generic;
using AutoMapper;
using DateApp.Core.DataModels;
using DateApp.Core.EntityModels;
using DateApp.DAL.Abstraction;

namespace DatingApp.Model.Mappers
{
    public class UserMapper: IUserMapper
    {
        private readonly IMapper _autoMapper;

        public UserMapper(IMapper autoMapper)
        {
            _autoMapper = autoMapper;
        }

        public UserDetailsDm MapEmToDm(User userEm)
        {
            return _autoMapper.Map<UserDetailsDm>(userEm);
        }
        
        public IEnumerable<UserListDm> MapEmsToDms(IEnumerable<User> userEms)
        {
            return _autoMapper.Map<IEnumerable<UserListDm>>(userEms);
        }

        public User MapDmToEm(UserUpdateDm userUpdateDm, User userFromRepo)
        {
            return _autoMapper.Map(userUpdateDm, userFromRepo);
        }
    }
}
