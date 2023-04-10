using Logitar.Portal.v2.Contracts.Sessions;
using Logitar.Portal.v2.Web.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.v2.Web.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
[Route("user")]
public class AccountController : Controller
{
  private readonly ISessionService _sessionService;

  public AccountController(ISessionService sessionService)
  {
    _sessionService = sessionService;
  }

  //[Authorize(Policy = Constants.Policies.AuthenticatedUser)] // TODO(fpion): Authorization
  [HttpGet("profile")]
  public ActionResult Profile()
  {
    return View(HttpContext.GetUser());
  }

  [HttpGet("sign-in")]
  public ActionResult SignIn()
  {
    if (HttpContext.IsSignedIn())
    {
      return RedirectToAction(actionName: "Profile");
    }

    return View();
  }

  //[Authorize(Policy = Constants.Policies.AuthenticatedUser)] // TODO(fpion): Authorization
  [HttpGet("sign-out")]
  public async Task<ActionResult> SignOut(CancellationToken cancellationToken)
  {
    await _sessionService.SignOutAsync(HttpContext.GetSessionId()!.Value, cancellationToken);

    HttpContext.Session.Clear();
    HttpContext.Response.Cookies.Delete(WebConstants.Cookies.RefreshToken);

    return RedirectToAction(actionName: "SignIn");
  }
}
