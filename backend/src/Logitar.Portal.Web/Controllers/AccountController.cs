using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Contracts.Errors;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Web.Constants;
using Logitar.Portal.Web.Extensions;
using Logitar.Portal.Web.Models.Account;
using Logitar.Portal.Web.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Web.Controllers;

[ApiController]
[Route("api/account")]
public class AccountController : ControllerBase
{
  private readonly ISessionService _sessionService;
  private readonly IUserService _userService;

  protected new User User => HttpContext.GetUser() ?? throw new InvalidOperationException("The User is required.");

  public AccountController(ISessionService sessionService, IUserService userService)
  {
    _sessionService = sessionService;
    _userService = userService;
  }

  [Authorize(Policy = Policies.PortalUser)]
  [HttpGet("profile")]
  public ActionResult<User> GetProfile() => Ok(User);

  [Authorize(Policy = Policies.PortalUser)]
  [HttpPatch("profile")]
  public async Task<ActionResult<User>> SaveProfileAsync([FromBody] UpdateProfileModel model, CancellationToken cancellationToken)
  {
    UpdateUserPayload payload = model.ToPayload();
    return Ok(await _userService.UpdateAsync(User.Id, payload, cancellationToken) ?? throw new InvalidOperationException("The User is required."));
  }

  [HttpPost("sign/in")]
  public async Task<ActionResult<CurrentUser>> SignInAsync([FromBody] SignInModel model, CancellationToken cancellationToken)
  {
    try
    {
      SignInSessionPayload payload = model.ToPayload(HttpContext.GetSessionCustomAttributes());
      Session session = await _sessionService.SignInAsync(payload, cancellationToken);
      HttpContext.SignIn(session);

      return Ok(new CurrentUser(session));
    }
    catch (InvalidCredentialsException)
    {
      return BadRequest(new Error("InvalidCredentials", InvalidCredentialsException.ErrorMessage));
    }
  }

  [Authorize(Policy = Policies.PortalUser)]
  [HttpPost("sign/out/all")]
  public async Task<ActionResult> SignOutAllAsync(CancellationToken cancellationToken)
  {
    _ = await _userService.SignOutAsync(User.Id, cancellationToken);
    HttpContext.SignOut();

    return NoContent();
  }

  [Authorize(Policy = Policies.PortalUser)]
  [HttpPost("sign/out")]
  public async Task<ActionResult> SignOutAsync(CancellationToken cancellationToken)
  {
    Guid? sessionId = HttpContext.GetSessionId();
    if (sessionId.HasValue)
    {
      _ = await _sessionService.SignOutAsync(sessionId.Value, cancellationToken);
    }
    HttpContext.SignOut();

    return NoContent();
  }
}
