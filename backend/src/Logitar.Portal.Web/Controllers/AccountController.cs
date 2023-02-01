using Logitar.Portal.Application.Accounts;
using Logitar.Portal.Contracts.Users.Models;
using Logitar.Portal.Web.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Web.Controllers
{
  [ApiExplorerSettings(IgnoreApi = true)]
  [Route("user")]
  public class AccountController : Controller
  {
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
      _accountService = accountService;
    }

    [Authorize(Policy = Constants.Policies.AuthenticatedUser)]
    [HttpGet("profile")]
    public async Task<ActionResult> Profile(CancellationToken cancellationToken)
    {
      UserModel user = await _accountService.GetProfileAsync(cancellationToken);

      return View(user);
    }

    [HttpGet("sign-in")]
    public ActionResult SignIn()
    {
      if (HttpContext.GetSessionId() != null)
      {
        return RedirectToAction(actionName: "Profile");
      }

      return View();
    }

    [Authorize(Policy = Constants.Policies.AuthenticatedUser)]
    [HttpGet("sign-out")]
    public async Task<ActionResult> SignOut(CancellationToken cancellationToken)
    {
      await _accountService.SignOutAsync(cancellationToken);

      HttpContext.Session.Clear();
      HttpContext.Response.Cookies.Delete(Constants.Cookies.RenewToken);

      return RedirectToAction(actionName: "SignIn");
    }
  }
}
