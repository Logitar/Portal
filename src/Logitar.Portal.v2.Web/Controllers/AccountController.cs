using Logitar.Portal.v2.Web.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.v2.Web.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
[Route("user")]
public class AccountController : Controller
{
  // TODO(fpion): Account Profile
  //[Authorize(Policy = Constants.Policies.AuthenticatedUser)]
  //[HttpGet("profile")]
  //public async Task<ActionResult> Profile(CancellationToken cancellationToken)
  //{
  //  User user = await _accountService.GetProfileAsync(cancellationToken);

  //  return View(user);
  //}

  [HttpGet("sign-in")]
  public ActionResult SignIn()
  {
    if (HttpContext.IsSignedIn())
    {
      return RedirectToAction(actionName: "Profile");
    }

    return View();
  }

  // TODO(fpion): Account SignOut
  //[Authorize(Policy = Constants.Policies.AuthenticatedUser)]
  //[HttpGet("sign-out")]
  //public async Task<ActionResult> SignOut(CancellationToken cancellationToken)
  //{
  //  await _accountService.SignOutAsync(cancellationToken);

  //  HttpContext.Session.Clear();
  //  HttpContext.Response.Cookies.Delete(Constants.Cookies.RenewToken);

  //  return RedirectToAction(actionName: "SignIn");
  //}
}
