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
  private readonly IUserService _userService;

  public AccountController(ISessionService sessionService, IUserService userService)
  {
    _sessionService = sessionService;
    _userService = userService;
  }

  private new User User => HttpContext.GetUser() ?? throw new InvalidOperationException("The User is required.");

  [Authorize]
  [HttpGet("profile")]
  public ActionResult<User> GetProfile()
  {
    return Ok(User);
  }

  [Authorize]
  [HttpPatch("profile")]
  public async Task<ActionResult<User>> SaveProfileAsync([FromBody] UpdateUserPayload payload, CancellationToken cancellationToken)
  {
    try
    {
      #region TODO(fpion): refactor
      if (payload.Password != null)
      {
        payload.Password.CurrentPassword ??= string.Empty;
      }
      payload.IsDisabled = null;
      #endregion

      User user = await _userService.UpdateAsync(User.Id, payload, cancellationToken) ?? throw new InvalidOperationException("The User is required.");
      return Ok(user);
    }
    catch (InvalidCredentialsException)
    {
      return BadRequest(new Error("InvalidCredentials", InvalidCredentialsException.ErrorMessage));
    }
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
  [HttpPost("sign/out/all")]
  public async Task<ActionResult> SignOutAllAsync(CancellationToken cancellationToken)
  {
    _ = await _userService.SignOutAsync(User.Id, cancellationToken);

    HttpContext.SignOut();

    return NoContent();
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
