using System;
using System.Linq;
using UserManagement.Models;

namespace UserManagement.Data.Tests;

public class DataContextTests
{
    // This test also tests the add functionality
    [Fact]
    public void GetAll_WhenNewEntityAdded_MustIncludeNewEntity()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var context = CreateContext();

        var entity = new User
        {
            Forename = "Brand New",
            Surname = "User",
            Email = "brandnewuser@example.com"
        };
        context.Create(entity);

        // Act: Invokes the method under test with the arranged parameters.
        var result = context.GetAll<User>();

        // Assert: Verifies that the action of the method under test behaves as expected.
        result
            .Should().Contain(s => s.Email == entity.Email)
            .Which.Should().BeEquivalentTo(entity);
    }

    // This test also tests the delete functionality
    [Fact]
    public void GetAll_WhenDeleted_MustNotIncludeDeletedEntity()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var context = CreateContext();
        var entity = context.GetAll<User>().First();
        context.Delete(entity);

        // Act: Invokes the method under test with the arranged parameters.
        var result = context.GetAll<User>();

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.Should().NotContain(s => s.Email == entity.Email);
    }

    [Fact]
    public void UpdateUser_MustUpdateDetails_ForCorrectUser()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var context = CreateContext();
        var updatedUser = context.GetAll<User>().First();
        updatedUser.Forename = "UpdatedForename";
        updatedUser.Surname = "UpdatedSurname";
        updatedUser.DateOfBirth = DateOnly.MinValue;
        updatedUser.IsActive = !updatedUser.IsActive; // We will always change this to ensure it can still be edited
        updatedUser.Email = Guid.NewGuid().ToString();

        context.Update(updatedUser);

        // Act: Invokes the method under test with the arranged parameters.
        var result = context.GetAll<User>().Where(x => x.Id == updatedUser.Id);

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.Should().Equal(updatedUser);
    }

    [Fact]
    public void UpdateUser_MustNotUpdateDetails_ForAllOtherUsers()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var context = CreateContext();

        var existingUsers = context.GetAll<User>();

        var updatedUser = existingUsers.First();
        updatedUser.Forename = "UpdatedForename";
        updatedUser.Surname = "UpdatedSurname";
        updatedUser.DateOfBirth = DateOnly.MinValue;
        updatedUser.IsActive = !updatedUser.IsActive; // We will always change this to ensure it can still be edited
        updatedUser.Email = Guid.NewGuid().ToString();

        context.Update(updatedUser);

        var newUsersList = existingUsers.Where(x => x.Id != updatedUser.Id).ToList();
        var serviceList = context.GetAll<User>().Where(x => x.Id != updatedUser.Id);

        newUsersList.Should().Equal(serviceList, because: "Only the updated user should have changed.");
    }

    private static DataContext CreateContext() => new();
}
