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

    [Fact]
    public void AddUser_WithExistingUser_CallsDataAccessCreate()
    {
        // Arrange
        var service = CreateService();
        SetupUsers();

        var newUser = new User()
        {
            Id = 999,
        };

        // Act
        service.AddUser(newUser);

        // Assert
        _dataContext.Verify(x => x.Create(newUser), Times.Once);
    }

    [Fact]
    public void AddUser_WithNullUser_ThrowsArgumentNullException()
    {
        var service = CreateService();

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        var act = () => service.AddUser(null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

        // Act and Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void UpdateUser_WillCallUpdateOnce()
    {
        var service = CreateService();
        var users = SetupUsers();

        var updatedUser = users.First();
        updatedUser.Forename = "UpdatedForename";
        updatedUser.Surname = "UpdatedSurname";
        updatedUser.DateOfBirth = DateOnly.MinValue;
        updatedUser.IsActive = !updatedUser.IsActive; // We will always change this to ensure it can still be edited
        updatedUser.Email = Guid.NewGuid().ToString();

        service.UpdateUser(updatedUser);

        _dataContext.Verify(x => x.Update(updatedUser), Times.Once);
    }


    [Fact]
    public void UpdateUser_WithNullUser_ThrowsArgumentNullException()
    {
        var service = CreateService();

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        var act = () => service.UpdateUser(null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

        // Act and Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void DeleteUserById_ExistingUserDoesNotThrow_AndCallsDeleteOnce()
    {
        var service = CreateService();
        var users = SetupUsers();

        var userId = users.First().Id;

        service.DeleteUserById(userId);

        _dataContext.Verify(x => x.Delete(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public void DeleteUserById_FakeUserThrowsNotFoundError_AndDoesntCallDelete()
    {
        var service = CreateService();
        var users = SetupUsers();

        var userId = 999;

        var act = () => service.DeleteUserById(userId);

        act.Should().Throw<KeyNotFoundException>($"User with ID {userId} not found");

        _dataContext.Verify(x => x.Delete(It.IsAny<User>()), Times.Never);
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
                Id = i,
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
