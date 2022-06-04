using Sabio.Models.Domain.Users;
using Sabio.Models.Requests.Users;
using System.Collections.Generic;

namespace Sabio.Services
{
    public interface IUserServiceV1
    {
        int Add(UserAddRequest userModel);
        void Delete(int Id);
        User Get(int id);
        List<User> GetAll();
        void Update(UserUpdateRequest userModel);
    }
}