using System.Collections.Generic;
using System;
using System.Linq;
using UserManagement.Models;
using UserManagement.Services.Domain.Implementations;

namespace UserManagement.Data.Tests;

public class UserServiceTests
{
    [Fact]
    public void GetAll_WhenContextReturnsEntities_MustReturnSameEntities()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var service = CreateService();
        var users = SetupUsers();

        // Act: Invokes the method under test with the arranged parameters.
        var result = service.GetAll();

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.Should().BeSameAs(users);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void FilterByActive_WhenContextReturnsEntities_MustReturnSameEntities_DependingOnIsActive(bool isActive)
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var service = CreateService();
        var users = SetupUsers();

        // Act: Invokes the method under test with the arranged parameters.
        var result = service.FilterByActive(isActive);

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.Should().BeEquivalentTo(users.Where(x => x.IsActive == isActive));
    }

    private IQueryable<User> SetupUsers()
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

        var usersAsQueryable = users.AsQueryable();

        _dataContext
            .Setup(s => s.GetAll<User>())
            .Returns(usersAsQueryable);

        return usersAsQueryable;
    }

    private readonly Mock<IDataContext> _dataContext = new();
    private UserService CreateService() => new(_dataContext.Object);
}
