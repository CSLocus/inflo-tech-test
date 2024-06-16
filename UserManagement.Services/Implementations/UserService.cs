using System;
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

    public User? GetUserById(long id)
    {
        ArgumentNullException.ThrowIfNull(id);
        return dataAccess.GetAll<User>().Where(x => x.Id == id).FirstOrDefault();
    }

    public IEnumerable<User> GetAll() => dataAccess.GetAll<User>();

    public void AddUser(User newUser)
    {
        ArgumentNullException.ThrowIfNull(newUser);
        dataAccess.Create(newUser);
    }

    public void UpdateUser(User updatedUser)
    {
        ArgumentNullException.ThrowIfNull(updatedUser);
        dataAccess.Update(updatedUser);
    }

    public void DeleteUserById(long userId)
    {
        ArgumentNullException.ThrowIfNull(userId);

        var user = GetUserById(userId);
        if (user is null) throw new KeyNotFoundException($"User with ID {userId} not found");

        dataAccess.Delete(user);
    }
}
