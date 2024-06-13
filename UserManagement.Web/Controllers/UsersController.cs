using System.Linq;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Web.Models.Users;

namespace UserManagement.WebMS.Controllers;

[Route("users")]
public class UsersController(IUserService userService) : Controller
{
    [HttpGet]
    public ViewResult List(bool? isActive = null)
    {
        IEnumerable<UserListItemViewModel> items;

        if (isActive.HasValue)
        {
            items = userService.FilterByActive(isActive.Value).Select(MapToViewModel);
        }
        else
        {
            items = userService.GetAll().Select(MapToViewModel);
        }

        ViewBag.IsActive = isActive;

        return View(new UserListViewModel { Items = items.ToList() });
    }

    [HttpGet("{userId}")]
    public ViewResult ViewUser(long userId)
    {
        var user = userService.GetUserById(userId);

        if (user is null)
        {
            return View("404");
        }

        var userViewModel = new UserViewModel()
        {
            Id = user.Id,
            Forename = user.Forename,
            Surname = user.Surname,
            Email = user.Email,
            IsActive = user.IsActive,
            DateOfBirth = user.DateOfBirth,
        };

        return View(userViewModel);
    }

    private static UserListItemViewModel MapToViewModel(User user) => new()
    {
        Id = user.Id,
        Forename = user.Forename,
        Surname = user.Surname,
        Email = user.Email,
        IsActive = user.IsActive,
        DateOfBirth = user.DateOfBirth,
    };
}
