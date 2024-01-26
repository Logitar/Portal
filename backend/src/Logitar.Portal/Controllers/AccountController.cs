using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Controllers;

[ApiController]
[Route("account")]
public class AccountController : ControllerBase
{
  [Authorize]
  [HttpGet("profile")]
  public ActionResult<User> GetProfile()
  {
    User user = HttpContext.GetUser() ?? throw new InvalidOperationException("The User is required.");
    return Ok(user);
  }
}
