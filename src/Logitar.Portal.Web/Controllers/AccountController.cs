using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Web.Constants;
using Logitar.Portal.Web.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Web.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
[Route("user")]
public class AccountController : Controller
{
  private readonly ISessionService _sessionService;

  public AccountController(ISessionService sessionService)
  {
    _sessionService = sessionService;
  }

  [Authorize(Policy = Policies.AuthenticatedPortalUser)]
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

  [Authorize(Policy = Policies.AuthenticatedPortalUser)]
  [HttpGet("sign-out")]
  public async Task<ActionResult> SignOut(CancellationToken cancellationToken)
  {
    await _sessionService.SignOutAsync(HttpContext.GetSessionId()!.Value, cancellationToken);
    HttpContext.SignOut();

    return RedirectToAction(actionName: "SignIn");
  }
}
