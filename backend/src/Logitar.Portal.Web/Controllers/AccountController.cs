using Logitar.Identity.Core;
using Logitar.Portal.Application.Activities;
using Logitar.Portal.Application.Sessions.Commands;
using Logitar.Portal.Application.Users.Commands;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Web.Constants;
using Logitar.Portal.Web.Extensions;
using Logitar.Portal.Web.Models.Account;
using Logitar.Portal.Web.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Web.Controllers;

[ApiController]
[Route("api/account")]
public class AccountController : ControllerBase
{
  private readonly IActivityPipeline _activityPipeline;

  protected ContextParameters Parameters => new()
  {
    User = User
  };
  protected new UserModel User => HttpContext.GetUser() ?? throw new InvalidOperationException("The User is required.");

  public AccountController(IActivityPipeline activityPipeline)
  {
    _activityPipeline = activityPipeline;
  }

  [Authorize(Policy = Policies.PortalUser)]
  [HttpGet("profile")]
  public ActionResult<UserModel> GetProfile() => Ok(User);

  [Authorize(Policy = Policies.PortalUser)]
  [HttpPatch("profile")]
  public async Task<ActionResult<UserModel>> SaveProfileAsync([FromBody] UpdateProfilePayload model, CancellationToken cancellationToken)
  {
    UpdateUserPayload payload = model.ToPayload();
    UpdateUserCommand command = new(User.Id, payload);
    return Ok(await _activityPipeline.ExecuteAsync(command, Parameters, cancellationToken) ?? throw new InvalidOperationException("The User is required."));
  }

  [HttpPost("sign/in")]
  public async Task<ActionResult<SessionModel>> SignInAsync([FromBody] SignInModel model, CancellationToken cancellationToken)
  {
    try
    {
      SignInSessionPayload payload = model.ToPayload(HttpContext.GetSessionCustomAttributes());
      SignInSessionCommand command = new(payload);
      SessionModel session = await _activityPipeline.ExecuteAsync(command, new ContextParameters(), cancellationToken);
      HttpContext.SignIn(session);

      return Ok(session);
    }
    catch (InvalidCredentialsException)
    {
      Error error = new(code: "InvalidCredentials", message: "The specified credentials did not match.");
      return Problem(
        detail: error.Message,
        instance: Request.GetDisplayUrl(),
        statusCode: StatusCodes.Status400BadRequest,
        title: "Invalid Credentials",
        type: null,
        extensions: new Dictionary<string, object?>
        {
          ["code"] = error.Code,
          ["message"] = error.Message,
          ["data"] = error.Data
        });
    }
  }

  [Authorize(Policy = Policies.PortalUser)]
  [HttpPost("sign/out/all")]
  public async Task<ActionResult> SignOutAllAsync(CancellationToken cancellationToken)
  {
    SignOutUserCommand command = new(User.Id);
    _ = await _activityPipeline.ExecuteAsync(command, Parameters, cancellationToken);
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
      SignOutSessionCommand command = new(sessionId.Value);
      _ = await _activityPipeline.ExecuteAsync(command, Parameters, cancellationToken);
    }
    HttpContext.SignOut();

    return NoContent();
  }
}
