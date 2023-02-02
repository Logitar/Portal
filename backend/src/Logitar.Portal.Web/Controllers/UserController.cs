using Logitar.Portal.Application.Users;
using Logitar.Portal.Contracts.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    public async Task<ActionResult> UserEdit(string id, CancellationToken cancellationToken = default)
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
