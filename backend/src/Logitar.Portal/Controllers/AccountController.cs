using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Controllers;

[ApiController]
[Route("account")]
public class AccountController : ControllerBase
{
  [HttpGet("profile")] // TODO(fpion): Authorization
  public ActionResult<User> GetProfile()
  {
    User user = HttpContext.GetUser() ?? throw new InvalidOperationException("The User is required.");
    return Ok(user);
  }
}
