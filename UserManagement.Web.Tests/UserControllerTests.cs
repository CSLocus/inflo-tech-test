using System;
using System.Collections.Generic;
using System.Linq;
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
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var controller = CreateController();
        var users = SetupUsers();

        // Act: Invokes the method under test with the arranged parameters.
        var result = controller.List(isActive);

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.Model
            .Should().BeOfType<UserListViewModel>()
            .Which.Items.Should().BeEquivalentTo(users.Where(x => !isActive.HasValue || x.IsActive == isActive));
    }

    private List<User> SetupUsers()
    {
        var users = new List<User>();
        var random = new Random();

        // We need to start at 1 so that random DoBs can be generated
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

        _userService
            .Setup(s => s.GetAll())
            .Returns(users);

        _userService
            .Setup(s => s.FilterByActive(true))
            .Returns(users.Where(x => x.IsActive));

        _userService
            .Setup(s => s.FilterByActive(false))
            .Returns(users.Where(x => !x.IsActive));

        return users;
    }

    private readonly Mock<IUserService> _userService = new();
    private UsersController CreateController() => new(_userService.Object);
}
