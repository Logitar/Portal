using Logitar.Portal.Contracts.Constants;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Web.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Web.Controllers;

[ApiController]
[Route("api/account")]
public class AccountController : ControllerBase
{
  private readonly ISessionService _sessionService;
  private readonly IUserService _userService;

  public AccountController(ISessionService sessionService, IUserService userService)
  {
    _sessionService = sessionService;
    _userService = userService;
  }

  [Authorize(Policy = Policies.PortalActor)]
  [HttpGet("profile")]
  public ActionResult<User> GetProfile()
  {
    User user = HttpContext.GetUser()
      ?? throw new InvalidOperationException("The User context item is required.");

    return Ok(user);
  }

  [Authorize(Policy = Policies.PortalActor)]
  [HttpPatch("profile")]
  public async Task<ActionResult<User>> SaveProfileAsync([FromBody] UpdateUserPayload payload, CancellationToken cancellationToken)
  {
    Guid id = HttpContext.GetUser()?.Id
      ?? throw new InvalidOperationException("The User context item is required.");
    User user = await _userService.UpdateAsync(id, payload, cancellationToken)
      ?? throw new InvalidOperationException("The user update operation returned null.");

    return Ok(user);
  }

  [HttpPost("sign/in")]
  public async Task<ActionResult<Session>> SignInAsync([FromBody] SignInPayload payload, CancellationToken cancellationToken)
  {
    payload.Realm = null;
    payload.CustomAttributes = HttpContext.GetSessionCustomAttributes();
    Session session = await _sessionService.SignInAsync(payload, cancellationToken);
    Uri uri = new($"{Request.Scheme}://{Request.Host}/api/sessions/{session.Id}");

    HttpContext.SignIn(session);

    return Created(uri, session);
  }

  [Authorize(Policy = Policies.PortalActor)]
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
