using Logitar.Portal.v2.Contracts.Users;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.v2.Web.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
//[Authorize(Policy = Constants.Policies.PortalIdentity)] // TODO(fpion): Authorization
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
    User? user = await _userService.GetAsync(id, cancellationToken: cancellationToken);
    if (user == null)
    {
      return NotFound();
    }

    return View(user);
  }
}
