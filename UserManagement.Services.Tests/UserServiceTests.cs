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
        var service = CreateService();
        var users = SetupUsers();

        var result = service.GetAll();

        result.Should().BeSameAs(users);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void FilterByActive_WhenContextReturnsEntities_MustReturnSameEntities_DependingOnIsActive(bool isActive)
    {
        var service = CreateService();
        var users = SetupUsers();

        var result = service.FilterByActive(isActive);

        result.Should().BeEquivalentTo(users.Where(x => x.IsActive == isActive));
    }


    [Fact]
    public void GetUserById_WhenContextReturnsUser_MustReturnUser()
    {
        var service = CreateService();
        var users = SetupUsers();

        var result = service.GetUserById(users.First().Id);

        result.Should().Be(users.First());
    }

    [Fact]
    public void GetUserById_WhenContextReturnsNull_MustReturnNull()
    {
        var service = CreateService();
        SetupUsers();

        var result = service.GetUserById(999);

        result.Should().BeNull();
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
