using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Web.Models.Users;
using UserManagement.WebMS.Controllers;

namespace UserManagement.Data.Tests;

public class UserControllerTests
{
    [Theory]
    [InlineData(null)]
    [InlineData(true)]
    [InlineData(false)]
    public void List_WhenServiceReturnsUsersDependingOnIsActive_ModelMustContainAppropriateUsers(bool? isActive)
    {
        var controller = CreateController();
        var users = SetupUsers();

        // We only need to set up the mocks for things we actually use
        if (isActive.HasValue)
        {
            _userService
                .Setup(s => s.FilterByActive(isActive.Value))
                .Returns(users.Where(x => x.IsActive == isActive));
        }
        else
        {
            _userService
                .Setup(s => s.GetAll())
                .Returns(users);
        }

        var result = controller.List(isActive);

        result.Model
            .Should().BeOfType<UserListViewModel>()
            .Which.Items.Should().BeEquivalentTo(users.Where(x => !isActive.HasValue || x.IsActive == isActive));
    }

    [Fact]
    public void ViewUser_WhenServiceReturnsUser_ModelMustBeCorrectUser()
    {
        var controller = CreateController();
        var users = SetupUsers();

        _userService
            .Setup(s => s.GetUserById(users.First().Id))
            .Returns(users.First());

        var result = controller.ViewUser(users.First().Id);

        result.Model
            .Should().BeOfType<UserViewModel>()
            .Which.Should().BeEquivalentTo(users.First());
    }

    [Fact]
    public void ViewUser_WhenServiceReturnsNull_ModelMustBeCorrectUser()
    {
        var controller = CreateController();
        var users = SetupUsers();

        _userService
            .Setup(s => s.GetUserById(users.First().Id))
            .Returns(null as User);

        var result = controller.ViewUser(users.First().Id);

        // Ensure that we are not loading the edit user page, and instead are displaying the 404 error
        result.Model.Should().BeNull();
        result.ViewName.Should().Be("404");
    }

    private static List<User> SetupUsers()
    {
        var users = new List<User>();
        var random = new Random();

        // We need to start at 1 so that DoBs can be generated dynamically
        for (var i = 1; i < 11; i++)
        {
            users.Add(new User
            {
                Forename = "Forename-" + Guid.NewGuid().ToString(), // Create random forename
                Surname = "Surname-" + Guid.NewGuid().ToString(), // Create random surname
                Email = $"testuser{i}@test.com", // Create email containing counter
                IsActive = random.NextDouble() >= 0.5, // Random active status so that the data isn't biased
                DateOfBirth = new DateOnly(2000 + i, i, i) // Create date of birth based on the counter
            });
        }

        return users;
    }

    private readonly Mock<IUserService> _userService = new();
    private UsersController CreateController() => new(_userService.Object);
}
