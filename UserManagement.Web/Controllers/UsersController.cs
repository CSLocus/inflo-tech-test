using System.Linq;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Web.Models.Users;

namespace UserManagement.WebMS.Controllers;

[Route("users")]
public class UsersController(IUserService userService) : Controller
{
    [HttpGet]
    public ViewResult List(bool? isActive)
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

        return View(new UserListViewModel { Items = items.ToList() });
    }

    private static UserListItemViewModel MapToViewModel(User user) => new()
    {
        Id = user.Id,
        Forename = user.Forename,
        Surname = user.Surname,
        Email = user.Email,
        IsActive = user.IsActive
    };
}
