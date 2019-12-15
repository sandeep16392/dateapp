using DateApp.Core.DataModels;
using DateApp.Core.EntityModels;
using System.Collections.Generic;

namespace DateApp.DAL.Abstraction
{
    public interface IUserMapper
    {
        UserDetailsDm MapEmToDm(User userEm);
        IEnumerable<UserListDm> MapEmsToDms(IEnumerable<User> userEms);
        User MapDmToEm(UserUpdateDm userUpdateDm, User userFromRepo);
    }
}
