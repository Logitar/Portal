using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Contracts.Errors;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Extensions;
using Logitar.Portal.Models.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Controllers;

[ApiController]
[Route("account")]
public class AccountController : ControllerBase
{
  private readonly ISessionService _sessionService;

  public AccountController(ISessionService sessionService)
  {
    _sessionService = sessionService;
  }

  [Authorize]
  [HttpGet("profile")]
  public ActionResult<User> GetProfile()
  {
    User user = HttpContext.GetUser() ?? throw new InvalidOperationException("The User is required.");
    return Ok(user);
  }

  [HttpPost("sign/in")]
  public async Task<ActionResult<CurrentUser>> SignInAsync([FromBody] SignInSessionModel model, CancellationToken cancellationToken)
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

  [Authorize]
  [HttpPost("sign/out")]
  public async Task<ActionResult> SignOutAsync(CancellationToken cancellationToken)
  {
    string? sessionId = HttpContext.GetSessionId();
    if (sessionId != null)
    {
      _ = await _sessionService.SignOutAsync(sessionId, cancellationToken);
    }

    HttpContext.SignOut();

    return NoContent();
  }
}
