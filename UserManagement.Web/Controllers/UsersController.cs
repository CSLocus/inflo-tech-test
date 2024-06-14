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

    [HttpGet("/add-user")]
    public ViewResult ViewAddUserScreen()
    {
        return View("AddUser");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult AddUser(EditUserModel user)
    {
        if (ModelState.IsValid)
        {
            userService.AddUser(new User()
            {
                Forename = user.Forename,
                Surname = user.Surname,
                Email = user.Email,
                DateOfBirth = user.DateOfBirth,
                IsActive = user.IsActive,
            });

            return RedirectToAction("List");
        }

        return View(user);
    }

    [HttpGet("{userId}")]
    public ViewResult ViewEditUserPage(long userId)
    {
        var user = userService.GetUserById(userId);

        if (user is null)
        {
            return View("404");
        }

        var userViewModel = new EditUserModel()
        {
            Id = user.Id,
            Forename = user.Forename,
            Surname = user.Surname,
            Email = user.Email,
            IsActive = user.IsActive,
            DateOfBirth = user.DateOfBirth,
        };

        return View("EditUser", userViewModel);
    }

    [HttpPost("{userId}")]
    [ValidateAntiForgeryToken]
    public IActionResult EditUser(long userId, EditUserModel user)
    {
        if (userId != user.Id)
        {
            return View("404");
        }

        if (ModelState.IsValid)
        {
            userService.UpdateUser(new User()
            {
                Id = user.Id,
                Forename = user.Forename,
                Surname = user.Surname,
                Email = user.Email,
                DateOfBirth = user.DateOfBirth,
                IsActive = user.IsActive,
            });

            return RedirectToAction("List");
        }

        return View(user);
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
