using System.Collections.Generic;
using UserManagement.Models;

namespace UserManagement.Services.Domain.Interfaces;

public interface IUserService 
{
    IEnumerable<User> FilterByActive(bool isActive);
    IEnumerable<User> GetAll();
    User? GetUserById(long id);
    void AddUser(User newUser);
    void UpdateUser(User editedUser);
    void DeleteUserById(long userId);
}
