using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Logitar.Portal.Core.Users;
using Logitar.Portal.Core.Users.Models;

namespace Logitar.Portal.Web.Controllers
{
  [ApiExplorerSettings(IgnoreApi = true)]
  [Authorize(Policy = Constants.Policies.PortalIdentity)]
  [Route("users")]
  public class UserController : Controller
  {
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
      _userService = userService;
    }

    [HttpGet("/create-user")]
    public ActionResult CreateUser()
    {
      return View(nameof(UserEdit));
    }

    [HttpGet]
    public ActionResult UserList()
    {
      return View();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> UserEdit(Guid id, CancellationToken cancellationToken = default)
    {
      UserModel? user = await _userService.GetAsync(id, cancellationToken);
      if (user == null)
      {
        return NotFound();
      }

      return View(user);
    }
  }
}
