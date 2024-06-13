using System.Collections.Generic;
using System.Linq;
using UserManagement.Data;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;

namespace UserManagement.Services.Domain.Implementations;

public class UserService(IDataContext dataAccess) : IUserService
{
    public IEnumerable<User> FilterByActive(bool isActive)
    {
        return dataAccess.GetAll<User>().Where(x => x.IsActive == isActive);
    }

    public User? GetUserById(int id)
    {
        return dataAccess.GetAll<User>().Where(x => x.Id == id).FirstOrDefault();
    }

    public IEnumerable<User> GetAll() => dataAccess.GetAll<User>();
}
